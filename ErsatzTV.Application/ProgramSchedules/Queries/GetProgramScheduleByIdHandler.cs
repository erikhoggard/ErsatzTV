using ErsatzTV.Core.Domain;
using ErsatzTV.Infrastructure.Data;
using ErsatzTV.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using static ErsatzTV.Application.ProgramSchedules.Mapper;

namespace ErsatzTV.Application.ProgramSchedules;

public class GetProgramScheduleByIdHandler :
    IRequestHandler<GetProgramScheduleById, Option<ProgramScheduleViewModel>>
{
    private readonly IDbContextFactory<TvContext> _dbContextFactory;

    public GetProgramScheduleByIdHandler(IDbContextFactory<TvContext> dbContextFactory) =>
        _dbContextFactory = dbContextFactory;

    public async Task<Option<ProgramScheduleViewModel>> Handle(
        GetProgramScheduleById request,
        CancellationToken cancellationToken)
    {
        await using TvContext dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.ProgramSchedules
            .AsNoTracking()
            .Include(ps => ps.LoadDistributions)
            .ThenInclude(ld => ld.MediaItem)
            .ThenInclude(mi => ((Show)mi).ShowMetadata)
            .Include(ps => ps.LoadDistributions)
            .ThenInclude(ld => ld.Collection)
            .Include(ps => ps.LoadDistributions)
            .ThenInclude(ld => ld.MultiCollection)
            .Include(ps => ps.LoadDistributions)
            .ThenInclude(ld => ld.SmartCollection)
            .SelectOneAsync(ps => ps.Id, ps => ps.Id == request.Id, cancellationToken)
            .MapT(ProjectToViewModel);
    }
}
