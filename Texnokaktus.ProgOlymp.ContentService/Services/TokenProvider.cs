using Microsoft.Kiota.Abstractions.Authentication;
namespace Texnokaktus.ProgOlymp.ContentService.Services;

public class TokenProvider : IAccessTokenProvider
{
    public Task<string> GetAuthorizationTokenAsync(Uri uri,
                                                   Dictionary<string, object>? additionalAuthenticationContext = null,
                                                   CancellationToken cancellationToken = default) =>
        Task.FromResult(AllowedHostsValidator.IsUrlHostValid(uri)
                            ? Environment.GetEnvironmentVariable("YANDEX_CONTEST_TOKEN") ?? throw new("No access token")
                            : string.Empty);

    public AllowedHostsValidator AllowedHostsValidator => new(["api.contest.yandex.net"]);
}
