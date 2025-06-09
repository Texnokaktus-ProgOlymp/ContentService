using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.ContentService.DataAccess.Context;
using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.ContentService.Models;
using Texnokaktus.ProgOlymp.ContentService.Queries;
using Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.ContentService.Handlers;

internal class ContentQueryHandler(AppDbContext context,
                                   IContentLinkGenerator contentLinkGenerator) : IContestContentQueryHandler,
                                                                                 IContestStageContentQueryHandler,
                                                                                 IContestProblemContentQueryHandler
{
    public Task<ContestContentItems> HandleAsync(ContestContentQuery query, CancellationToken cancellationToken = default) =>
        HandleAsync(contentItem => contentItem.ContestName == query.ContestName,
                    GetContestContentItems,
                    cancellationToken);

    public Task<ContestStageContentItems> HandleAsync(ContestStageContentQuery query, CancellationToken cancellationToken = default) =>
        HandleAsync(contentItem => contentItem.ContestName == query.ContestName
                                && contentItem.ContestStage == query.ContestStage,
                    GetContestStageContentItems,
                    cancellationToken);

    public Task<ContestProblemContentItems> HandleAsync(ContestProblemContentQuery query, CancellationToken cancellationToken = default) =>
        HandleAsync(contentItem => contentItem.ContestName == query.ContestName
                                && contentItem.ContestStage == query.ContestStage 
                                && contentItem.ProblemAlias == query.ProblemAlias,
                    GetContestProblemContentItems,
                    cancellationToken);

    private async Task<TResult> HandleAsync<TResult>(Expression<Func<ContentItem, bool>> predicate,
                                                         Func<ICollection<ContentItem>, TResult> selector,
                                                         CancellationToken cancellationToken)
    {
        var contentItems = await context.ContentItems
                                        .AsNoTracking()
                                        .Where(predicate)
                                        .ToArrayAsync(cancellationToken);

        return selector.Invoke(contentItems);
    }

    private ContestContentItems GetContestContentItems(ICollection<ContentItem> contentItems) =>
        new(contentItems.Where(static item => item.ContestStage is null)
                        .Select(contentItem => MapToContentItemModel(contentItem,
                                                                     "GetContestContentItem",
                                                                     static contentItem =>
                                                                         new()
                                                                         {
                                                                             ["contestName"] = contentItem.ContestName,
                                                                             ["shortName"] = contentItem.ShortName
                                                                         })),
            contentItems.Where(static contentItem => contentItem.ContestStage is not null)
                        .GroupBy(static contentItem => contentItem.ContestStage,
                                 (contestStage, items) => KeyValuePair.Create(contestStage!.Value,
                                                                              GetContestStageContentItems(items.ToArray())))
                        .ToDictionary());

    private ContestStageContentItems GetContestStageContentItems(ICollection<ContentItem> contentItems) =>
        new(contentItems.Where(static contentItem => contentItem.ProblemAlias is null)
                        .Select(contentItem => MapToContentItemModel(contentItem,
                                                                     "GetContestStageContentItem",
                                                                     static contentItem =>
                                                                         new()
                                                                         {
                                                                             ["contestName"] = contentItem.ContestName,
                                                                             ["contestStage"] = contentItem.ContestStage,
                                                                             ["shortName"] = contentItem.ShortName
                                                                         })),
            contentItems.Where(static contentItem => contentItem.ProblemAlias is not null)
                        .GroupBy(static contentItem => contentItem.ProblemAlias,
                                 (problemAlias, items) => KeyValuePair.Create(problemAlias!,
                                                                              GetContestProblemContentItems(items)))
                        .ToDictionary());

    private ContestProblemContentItems GetContestProblemContentItems(IEnumerable<ContentItem> contentItems) =>
        new(contentItems.Select(contentItem => MapToContentItemModel(contentItem,
                                                                     "GetContestProblemContentItem",
                                                                     static contentItem =>
                                                                         new()
                                                                         {
                                                                             ["contestName"] = contentItem.ContestName,
                                                                             ["contestStage"] = contentItem.ContestStage,
                                                                             ["problemAlias"] = contentItem.ProblemAlias,
                                                                             ["shortName"] = contentItem.ShortName
                                                                         })));

    private ContentItemModel MapToContentItemModel(ContentItem contentItem, string routeName, Func<ContentItem, RouteValueDictionary> linkDataProvider) =>
        new(contentItem.Description,
            contentLinkGenerator.GetPath(routeName, linkDataProvider.Invoke(contentItem)));
}
