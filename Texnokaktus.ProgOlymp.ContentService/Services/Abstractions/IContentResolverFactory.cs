using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;

public interface IContentResolverFactory
{
    IContentResolver GetFor(ContentItem item);
}
