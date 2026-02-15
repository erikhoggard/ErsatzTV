using ErsatzTV.Core;
using ErsatzTV.Core.Domain;
using ErsatzTV.Infrastructure.Data;
using ErsatzTV.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ErsatzTV.Application.ProgramSchedules;

public class ReplaceProgramScheduleLoadDistributionsHandler(IDbContextFactory<TvContext> dbContextFactory)
    : IRequestHandler<ReplaceProgramScheduleLoadDistributions, Either<BaseError, Unit>>
{
    public async Task<Either<BaseError, Unit>> Handle(
        ReplaceProgramScheduleLoadDistributions request,
        CancellationToken cancellationToken)
    {
        await using TvContext dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        Validation<BaseError, ProgramSchedule> validation = await Validate(dbContext, request, cancellationToken);
        return await validation.Apply(ps => ApplyReplaceRequest(dbContext, ps, request));
    }

    private static async Task<Unit> ApplyReplaceRequest(
        TvContext dbContext,
        ProgramSchedule programSchedule,
        ReplaceProgramScheduleLoadDistributions request)
    {
        programSchedule.LoadDistributions.Clear();

        programSchedule.LoadDistributions = request.LoadDistributions.Select(
            item => new ProgramScheduleLoadDistribution
            {
                ProgramScheduleId = programSchedule.Id,
                MediaItemId = item.MediaItemId,
                CollectionId = item.CollectionId,
                MultiCollectionId = item.MultiCollectionId,
                SmartCollectionId = item.SmartCollectionId,
                Weight = item.Weight
            }).ToList();

        // Automatically enable custom probabilities when distributions are saved
        programSchedule.UseCustomProbabilities = true;

        await dbContext.SaveChangesAsync();

        return Unit.Default;
    }

    private static Task<Validation<BaseError, ProgramSchedule>> Validate(
        TvContext dbContext,
        ReplaceProgramScheduleLoadDistributions request,
        CancellationToken cancellationToken) =>
        ProgramScheduleMustExist(dbContext, request, cancellationToken);

    private static Task<Validation<BaseError, ProgramSchedule>> ProgramScheduleMustExist(
        TvContext dbContext,
        ReplaceProgramScheduleLoadDistributions request,
        CancellationToken cancellationToken) =>
        dbContext.ProgramSchedules
            .Include(ps => ps.LoadDistributions)
            .SelectOneAsync(ps => ps.Id, ps => ps.Id == request.ProgramScheduleId, cancellationToken)
            .Map(o => o.ToValidation<BaseError>("Schedule does not exist"));
}
