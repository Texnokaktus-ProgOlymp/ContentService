namespace Texnokaktus.ProgOlymp.ContentService.Handlers;

public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
