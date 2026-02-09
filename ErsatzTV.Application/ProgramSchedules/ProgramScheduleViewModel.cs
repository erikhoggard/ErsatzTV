using ErsatzTV.Core.Scheduling;

namespace ErsatzTV.Application.ProgramSchedules;

public record ProgramScheduleViewModel(
    int Id,
    string Name,
    bool KeepMultiPartEpisodesTogether,
    bool TreatCollectionsAsShows,
    bool ShuffleScheduleItems,
    bool RandomStartPoint,
    bool UseCustomProbabilities,
    FixedStartTimeBehavior FixedStartTimeBehavior,
    List<ProgramScheduleLoadDistributionViewModel> LoadDistributions);
