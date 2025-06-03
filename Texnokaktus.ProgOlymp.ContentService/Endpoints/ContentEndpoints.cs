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
                                                                                cancellationToken));

        app.MapGet("contests/{contestName}/{contestStage}/content/{shortName}",
                   (string contestName,
                    ContestStage contestStage,
                    string shortName,
                    IContentItemQueryHandler handler,
                    CancellationToken cancellationToken) => handler.HandleAsync(new(contestName,
                                                                                    contestStage,
                                                                                    null,
                                                                                    shortName),
                                                                                cancellationToken));

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
                                                                                cancellationToken));

        return app;
    }

    public static IEndpointRouteBuilder MapContentApiEndpoints(this IEndpointRouteBuilder app)
    {
        return app;
    }
}
