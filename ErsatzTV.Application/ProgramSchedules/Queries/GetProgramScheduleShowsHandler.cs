using ErsatzTV.Application.MediaItems;
using ErsatzTV.Core;
using ErsatzTV.Core.Domain;
using ErsatzTV.Core.Interfaces.Repositories;
using ErsatzTV.Core.Scheduling;
using ErsatzTV.Infrastructure.Data;
using ErsatzTV.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ErsatzTV.Application.ProgramSchedules;

public class GetProgramScheduleShowsHandler(
    IDbContextFactory<TvContext> dbContextFactory,
    IMediaCollectionRepository mediaCollectionRepository,
    ITelevisionRepository televisionRepository,
    IArtistRepository artistRepository)
    : IRequestHandler<GetProgramScheduleShows, Either<BaseError, List<NamedMediaItemViewModel>>>
{
    public async Task<Either<BaseError, List<NamedMediaItemViewModel>>> Handle(
        GetProgramScheduleShows request,
        CancellationToken cancellationToken)
    {
        await using TvContext dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        Option<ProgramSchedule> maybeSchedule = await dbContext.ProgramSchedules
            .Include(ps => ps.Items)
            .SelectOneAsync(ps => ps.Id, ps => ps.Id == request.ProgramScheduleId, cancellationToken);

        return await maybeSchedule.Match<Task<Either<BaseError, List<NamedMediaItemViewModel>>>>(
            async ps =>
            {
                var shows = new Dictionary<int, NamedMediaItemViewModel>();
                
                foreach (ProgramScheduleItem item in ps.Items)
                {
                    var collectionKey = CollectionKey.ForScheduleItem(item);
                    List<MediaItem> items = await MediaItemsForCollection.Collect(
                        mediaCollectionRepository,
                        televisionRepository,
                        artistRepository,
                        collectionKey,
                        cancellationToken);

                    foreach (MediaItem mediaItem in items)
                    {
                        if (mediaItem is Episode e && e.Season != null)
                        {
                            int showId = e.Season.ShowId;
                            if (!shows.ContainsKey(showId))
                            {
                                // We might need to load the show metadata if it's not there
                                // But MediaItemsForCollection.Collect usually returns items with some level of data.
                                // Actually, for episodes, it might not have the Show title easily available without navigating.
                                
                                Option<Show> maybeShow = await televisionRepository.GetShow(showId, cancellationToken);
                                foreach (Show show in maybeShow)
                                {
                                    string title = show.ShowMetadata.HeadOrNone().Map(m => m.Title).IfNone("Unknown Show");
                                    shows[showId] = new NamedMediaItemViewModel(showId, title);
                                }
                            }
                        }
                    }
                }

                return shows.Values.OrderBy(s => s.Name).ToList();
            },
            () => Task.FromResult<Either<BaseError, List<NamedMediaItemViewModel>>>(BaseError.New("Schedule not found")));
    }
}
