using Microsoft.AspNetCore.Http.HttpResults;
using Texnokaktus.ProgOlymp.ContentService.Queries;

namespace Texnokaktus.ProgOlymp.ContentService.Handlers;

public interface IContentItemQueryHandler
{
    Task<Results<FileStreamHttpResult, NotFound>> HandleAsync(ContentQuery query, CancellationToken cancellationToken = default);
}
