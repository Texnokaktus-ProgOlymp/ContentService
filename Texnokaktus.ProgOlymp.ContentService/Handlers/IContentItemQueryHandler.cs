using Microsoft.AspNetCore.Http.HttpResults;
using Texnokaktus.ProgOlymp.ContentService.Queries;

namespace Texnokaktus.ProgOlymp.ContentService.Handlers;

public interface IContentItemQueryHandler : IQueryHandler<ContentQuery, Results<FileStreamHttpResult, NotFound>>;
