using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.ContentService.DataAccess.Context;
using Texnokaktus.ProgOlymp.ContentService.Queries;
using Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.ContentService.Handlers;

internal class ContentItemQueryHandler(AppDbContext context, IContentResolverFactory contentResolverFactory) : IContentItemQueryHandler
{
    public async Task<Results<FileStreamHttpResult, NotFound>> HandleAsync(ContentQuery query, CancellationToken cancellationToken = default)
    {
        if (await context.ContentItems.FirstOrDefaultAsync(item => item.ContestName == query.ContestName
                                                                && item.ContestStage == query.ContestStage
                                                                && item.ProblemAlias == query.ProblemAlias
                                                                && item.ShortName == query.ShortName,
                                                           cancellationToken) is not { } contentItem)
            return TypedResults.NotFound();

        var resolver = contentResolverFactory.GetFor(contentItem);

        if (await resolver.ResolveAsync(contentItem, cancellationToken) is not { } data)
            return TypedResults.NotFound();

        return TypedResults.File(data.Content, data.ContentType, data.FileName, data.LastModified);
    }
}
