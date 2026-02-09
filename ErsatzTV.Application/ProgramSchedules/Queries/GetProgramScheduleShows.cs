using ErsatzTV.Application.MediaItems;
using ErsatzTV.Core;

namespace ErsatzTV.Application.ProgramSchedules;

public record GetProgramScheduleShows(int ProgramScheduleId) : IRequest<Either<BaseError, List<NamedMediaItemViewModel>>>;
