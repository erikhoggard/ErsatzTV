namespace ErsatzTV.Application.ProgramSchedules;

public record ProgramScheduleLoadDistributionViewModel(
    int Id,
    int? MediaItemId,
    string MediaItemTitle,
    int? CollectionId,
    string CollectionName,
    int? MultiCollectionId,
    string MultiCollectionName,
    int? SmartCollectionId,
    string SmartCollectionName,
    int Weight);
