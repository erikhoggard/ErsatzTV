using ErsatzTV.Core.Domain;
using ErsatzTV.Core.Extensions;
using ErsatzTV.Core.Interfaces.Scheduling;

namespace ErsatzTV.Core.Scheduling;

public class WeightedMediaCollectionEnumerator : IMediaCollectionEnumerator
{
    private readonly CancellationToken _cancellationToken;
    private readonly Map<int, int> _weights;
    private readonly PlaybackOrder _playbackOrder;
    private readonly CloneableRandom _random;
    private readonly Lazy<Option<TimeSpan>> _lazyMinimumDuration;

    private readonly Dictionary<int, IMediaCollectionEnumerator> _enumerators;
    private readonly List<int> _activeBuckets;
    private Option<MediaItem> _current;
    private readonly int _totalCount;
    private int _selectedBucket;

    public WeightedMediaCollectionEnumerator(
        List<GroupedMediaItem> groupedItems,
        Map<int, int> weights,
        PlaybackOrder playbackOrder,
        CollectionEnumeratorState state,
        bool randomStartPoint,
        CancellationToken cancellationToken)
    {
        _weights = weights;
        _playbackOrder = playbackOrder;
        _cancellationToken = cancellationToken;
        _random = new CloneableRandom(state.Seed);

        _enumerators = new Dictionary<int, IMediaCollectionEnumerator>();
        _activeBuckets = new List<int>();
        _totalCount = groupedItems.Sum(gi => 1 + gi.Additional.Count);

        // Group grouped items by ShowId
        var groups = groupedItems.GroupBy(gi => gi.First switch
        {
            Episode e => e.Season?.ShowId ?? 0,
            _ => 0
        })
        .OrderBy(g => g.Key)
        .ToDictionary(g => g.Key, g => g.ToList());

        // Use different seeds for each bucket to ensure different shuffle orders
        var bucketRandom = new Random(state.Seed);

        foreach (var group in groups)
        {
            int bucketKey = group.Key;
            List<GroupedMediaItem> items = group.Value;
            List<MediaItem> flatItems = items.SelectMany(gi => gi.Additional.Prepend(gi.First)).ToList();

            // Each bucket gets its own seed derived from the main seed
            int bucketSeed = bucketRandom.Next();

            // Apply random start point: pick a random index within this bucket's items
            int startIndex = randomStartPoint && flatItems.Count > 1
                ? bucketRandom.Next(0, flatItems.Count - 1)
                : 0;

            IMediaCollectionEnumerator enumerator = playbackOrder switch
            {
                PlaybackOrder.Chronological => new ChronologicalMediaCollectionEnumerator(
                    flatItems,
                    new CollectionEnumeratorState { Seed = bucketSeed, Index = startIndex }),
                PlaybackOrder.SeasonEpisode => new SeasonEpisodeMediaCollectionEnumerator(
                    flatItems,
                    new CollectionEnumeratorState { Seed = bucketSeed, Index = startIndex }),
                PlaybackOrder.ShuffleInOrder => new ChronologicalMediaCollectionEnumerator(
                    flatItems,
                    new CollectionEnumeratorState { Seed = bucketSeed, Index = startIndex }),
                PlaybackOrder.Random => new RandomizedMediaCollectionEnumerator(
                    flatItems,
                    new CollectionEnumeratorState { Seed = bucketSeed, Index = startIndex }),
                _ => new ShuffledMediaCollectionEnumerator(
                    items,
                    new CollectionEnumeratorState { Seed = bucketSeed, Index = startIndex },
                    cancellationToken)
            };

            _enumerators[bucketKey] = enumerator;
            _activeBuckets.Add(bucketKey);
        }

        State = new CollectionEnumeratorState { Seed = state.Seed, Index = 0 };

        _lazyMinimumDuration = new Lazy<Option<TimeSpan>>(() =>
            _enumerators.Values.Bind(e => e.MinimumDuration).OrderBy(identity).HeadOrNone());

        // Initialize _current by selecting the first bucket without advancing
        if (_enumerators.Count > 0)
        {
            InitializeCurrent();
        }

        // Move to the requested index
        while (State.Index < state.Index)
        {
            MoveNext(Option<DateTimeOffset>.None);
        }
    }

    private void InitializeCurrent()
    {
        SelectNextBucket();
        if (_enumerators.TryGetValue(_selectedBucket, out var enumerator))
        {
            _current = enumerator.Current;
        }
    }

    private void SelectNextBucket()
    {
        if (_activeBuckets.Count == 0)
        {
            _activeBuckets.AddRange(_enumerators.Keys);
        }

        if (_activeBuckets.Count == 0)
        {
            return;
        }

        int totalWeight = 0;
        var weightsToUse = new Dictionary<int, int>();

        foreach (int bucketKey in _activeBuckets)
        {
            int weight = _weights.Find(bucketKey).IfNone(50);
            if (weight < 1) weight = 1;
            weightsToUse[bucketKey] = weight;
            totalWeight += weight;
        }

        if (totalWeight == 0)
        {
            _selectedBucket = _activeBuckets[0];
            return;
        }

        int r = _random.Next(totalWeight);
        int currentSum = 0;
        _selectedBucket = _activeBuckets[0];

        foreach (int bucketKey in _activeBuckets)
        {
            currentSum += weightsToUse[bucketKey];
            if (r < currentSum)
            {
                _selectedBucket = bucketKey;
                break;
            }
        }

    }

    public void ResetState(CollectionEnumeratorState state)
    {
        // Not fully supported for complex weighted state
    }

    public string SchedulingContextName => "Weighted";

    public CollectionEnumeratorState State { get; }

    public Option<MediaItem> Current => _current;

    public Option<bool> CurrentIncludeInProgramGuide => _current.Map(_ => true);

    public void MoveNext(Option<DateTimeOffset> scheduledAt)
    {
        if (_enumerators.Count == 0)
        {
            _current = Option<MediaItem>.None;
            return;
        }

        // Advance the currently selected bucket's enumerator
        // Note: We do NOT remove exhausted buckets - they should continue cycling
        // to maintain the configured weight distribution over time
        if (_enumerators.TryGetValue(_selectedBucket, out var currentEnumerator))
        {
            currentEnumerator.MoveNext(scheduledAt);
        }

        State.Index++;

        // Select next bucket and set _current
        SelectNextBucket();
        if (_enumerators.TryGetValue(_selectedBucket, out var nextEnumerator))
        {
            _current = nextEnumerator.Current;
        }
        else
        {
            _current = Option<MediaItem>.None;
        }
    }

    public Option<TimeSpan> MinimumDuration => _lazyMinimumDuration.Value;
    public int Count => _totalCount;
}
