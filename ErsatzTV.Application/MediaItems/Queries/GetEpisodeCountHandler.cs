using ErsatzTV.Core.Interfaces.Repositories;
using ErsatzTV.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErsatzTV.Application.MediaItems;

public class GetEpisodeCountHandler(IDbContextFactory<TvContext> dbContextFactory) : IRequestHandler<GetEpisodeCount, int>
{
    public async Task<int> Handle(GetEpisodeCount request, CancellationToken cancellationToken)
    {
        await using TvContext dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Episodes
            .AsNoTracking()
            .CountAsync(e => e.Season.ShowId == request.ShowId, cancellationToken);
    }
}
