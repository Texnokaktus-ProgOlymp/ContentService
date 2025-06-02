using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.ContentService.Models;

namespace Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;

public interface IContentResolver
{
    Task<ContentItemData?> ResolveAsync(ContentItem contentItem);
}

public interface IContentResolver<in TContentItem> : IContentResolver where TContentItem : ContentItem
{
    Task<ContentItemData?> IContentResolver.ResolveAsync(ContentItem contentItem)
    {
        if (contentItem is not TContentItem concreteContentItem) throw new InvalidOperationException();
        return ResolveAsync(concreteContentItem);
    }

    Task<ContentItemData?> ResolveAsync(TContentItem contentItem);
}
