using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.ContentService.Services;

public class ContentResolverFactory(IServiceProvider provider) : IContentResolverFactory
{
    public IContentResolver GetFor(ContentItem item) =>
        item switch
        {
            GitHubReleaseItem            => Create<GitHubReleaseItem>(),
            S3Item                       => Create<S3Item>(),
            YandexContestProblemTestItem => Create<YandexContestProblemTestItem>(),
            _                            => throw new NotSupportedException()
        };

    private IContentResolver<TContentResolver> Create<TContentResolver>() where TContentResolver : ContentItem =>
        provider.GetRequiredService<IContentResolver<TContentResolver>>();
}
