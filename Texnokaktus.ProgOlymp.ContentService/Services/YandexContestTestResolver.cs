using System.IO.Compression;
using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.ContentService.Models;
using Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;
using YandexContestClient.Client;
using YandexContestClient.Client.Models;

namespace Texnokaktus.ProgOlymp.ContentService.Services;

public class YandexContestProblemTestResolver(ContestClient client) : IContentResolver<YandexContestProblemTestItem>
{
    public async Task<ContentItemData?> ResolveAsync(YandexContestProblemTestItem contentItem, CancellationToken cancellationToken = default)
    {
        var problemIdParts = contentItem.ProblemId.Split('/');

        var problemSettings = await client.Problems[problemIdParts[0]][problemIdParts[1]][problemIdParts[2]]
                                          .GetAsync(contentItem.ProblemId,
                                                    cancellationToken: cancellationToken);

        var files = await client.Problems[problemIdParts[0]][problemIdParts[1]][problemIdParts[2]]
                                .Files.GetAsync(contentItem.ProblemId,
                                                cancellationToken: cancellationToken);

        var stream = new MemoryStream();

        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            foreach (var (fileName, file) in problemSettings?.Testsets?.SelectMany(testSet => testSet.Tests ?? [])
                                                             .SelectMany(x => new[] { x.Input!, x.Answer! })
                                                             .Join(GetPlainFileStructure(files?.Files ?? new()),
                                                                   fileName => fileName,
                                                                   pair => pair.Key,
                                                                   (_, pair) => pair)
                                          ?? [])
            {
                await using var entryStream = archive.CreateEntry(fileName, CompressionLevel.Optimal).Open();
                using var httpClient = new HttpClient();
                await using var fileStream = await httpClient.GetStreamAsync(file.Url, cancellationToken);
                await fileStream.CopyToAsync(entryStream, cancellationToken);
            }
        }

        stream.Seek(0, SeekOrigin.Begin);

        return new(stream,
                   contentItem.OverrideFileName ?? "data.zip",
                   contentItem.OverrideContentType ?? "application/zip",
                   DateTimeOffset.UtcNow);
    }

    private static IEnumerable<KeyValuePair<string, PresignedS3File>> GetPlainFileStructure(PresignedProblemFile root)
    {
        foreach (var presignedProblemFile in root.Children ?? [])
        {
            if (presignedProblemFile.IsDirectory is true)
            {
                foreach (var child in GetPlainFileStructure(presignedProblemFile))
                    yield return KeyValuePair.Create($"{presignedProblemFile.Name}/{child.Key}", child.Value);
            }
            else
            {
                yield return KeyValuePair.Create(presignedProblemFile.Name!, presignedProblemFile.File!);
            }
        }
    }
}
