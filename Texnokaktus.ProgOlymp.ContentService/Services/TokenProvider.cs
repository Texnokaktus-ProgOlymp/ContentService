using YandexContestClient.Authentication;

namespace Texnokaktus.ProgOlymp.ContentService.Services;

public class TokenProvider : AccessTokenProviderBase
{
    protected override Task<string> GetTokenAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Environment.GetEnvironmentVariable("YANDEX_CONTEST_TOKEN") ?? throw new("No access token"));
}
