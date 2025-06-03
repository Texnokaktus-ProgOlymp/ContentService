using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.ContentService.Handlers;

namespace Texnokaktus.ProgOlymp.ContentService.Endpoints;

public static class ContentEndpoints
{
    public static IEndpointRouteBuilder MapContentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("contests/{contestName}/content/{shortName}",
                   (string contestName,
                    string shortName,
                    IContentItemQueryHandler handler,
                    CancellationToken cancellationToken) => handler.HandleAsync(new(contestName,
                                                                                    null,
                                                                                    null,
                                                                                    shortName),
                                                                                cancellationToken))
           .WithName("GetContestContentItem");

        app.MapGet("contests/{contestName}/{contestStage}/content/{shortName}",
                   (string contestName,
                    ContestStage contestStage,
                    string shortName,
                    IContentItemQueryHandler handler,
                    CancellationToken cancellationToken) => handler.HandleAsync(new(contestName,
                                                                                    contestStage,
                                                                                    null,
                                                                                    shortName),
                                                                                cancellationToken))
           .WithName("GetContestStageContentItem");

        app.MapGet("contests/{contestName}/{contestStage}/problems/{problemAlias}/content/{shortName}",
                   (string contestName,
                    ContestStage contestStage,
                    string problemAlias,
                    string shortName,
                    IContentItemQueryHandler handler,
                    CancellationToken cancellationToken) => handler.HandleAsync(new(contestName,
                                                                                    contestStage,
                                                                                    problemAlias,
                                                                                    shortName),
                                                                                cancellationToken))
           .WithName("GetContestProblemContentItem");

        return app;
    }

    public static IEndpointRouteBuilder MapContentApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("contests/{contestName}/content",
                   (string contestName,
                    IContestContentQueryHandler handler,
                    CancellationToken cancellationToken) => handler.HandleAsync(new(contestName),
                                                                                cancellationToken));

        app.MapGet("contests/{contestName}/{contestStage}/content",
                   (string contestName,
                    ContestStage contestStage,
                    IContestStageContentQueryHandler handler,
                    CancellationToken cancellationToken) => handler.HandleAsync(new(contestName,
                                                                                    contestStage),
                                                                                cancellationToken));

        app.MapGet("contests/{contestName}/{contestStage}/problems/{problemAlias}/content",
                   (string contestName,
                    ContestStage contestStage,
                    string problemAlias,
                    IContestProblemContentQueryHandler handler,
                    CancellationToken cancellationToken) => handler.HandleAsync(new(contestName,
                                                                                    contestStage,
                                                                                    problemAlias),
                                                                                cancellationToken));

        return app;
    }
}
