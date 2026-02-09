using ErsatzTV.Core;

namespace ErsatzTV.Application.ProgramSchedules;

public record ReplaceProgramScheduleLoadDistribution(
    int? MediaItemId,
    int? CollectionId,
    int? MultiCollectionId,
    int? SmartCollectionId,
    int Weight);

public record ReplaceProgramScheduleLoadDistributions(
    int ProgramScheduleId,
    List<ReplaceProgramScheduleLoadDistribution> LoadDistributions) : IRequest<Either<BaseError, Unit>>;
