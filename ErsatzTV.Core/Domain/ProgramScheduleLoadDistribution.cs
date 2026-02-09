namespace ErsatzTV.Core.Domain;

public class ProgramScheduleLoadDistribution
{
    public int Id { get; set; }
    public int ProgramScheduleId { get; set; }
    public ProgramSchedule ProgramSchedule { get; set; }
    public int? MediaItemId { get; set; }
    public MediaItem MediaItem { get; set; }
    public int? CollectionId { get; set; }
    public Collection Collection { get; set; }
    public int? MultiCollectionId { get; set; }
    public MultiCollection MultiCollection { get; set; }
    public int? SmartCollectionId { get; set; }
    public SmartCollection SmartCollection { get; set; }
    public int Weight { get; set; }
}
