using Microsoft.AspNetCore.Http.HttpResults;

namespace Texnokaktus.ProgOlymp.ContentService.Handlers;

public interface IContentQueryHandler
{
    Task<Results<FileStreamHttpResult, NotFound>> HandleAsync(ContentQuery query, CancellationToken cancellationToken = default);
}
