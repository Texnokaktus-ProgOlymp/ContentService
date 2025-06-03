using Octokit;
using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.ContentService.Models;
using Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.ContentService.Services;

public class GitHubReleaseContentResolver(GitHubClient gitHubClient) : IContentResolver<GitHubReleaseItem>
{
    public async Task<ContentItemData?> ResolveAsync(GitHubReleaseItem contentItem, CancellationToken cancellationToken = default)
    {
        var release = await gitHubClient.Repository.Release.GetLatest(contentItem.OwnerName, contentItem.RepositoryName);

        if (release?.Assets?.FirstOrDefault(asset => asset.Name == contentItem.AssetName) is not { } releaseAsset)
            return null;

        var response = await gitHubClient.Connection.Get<Stream>(new(releaseAsset.Url),
                                                                 new Dictionary<string, string>(),
                                                                 "application/octet-stream",
                                                                 cancellationToken);

        return new(response.Body,
                   contentItem.OverrideFileName ?? contentItem.AssetName,
                   contentItem.OverrideContentType ?? releaseAsset.ContentType,
                   releaseAsset.UpdatedAt);
    }
}
