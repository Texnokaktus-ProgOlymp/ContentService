using System.Net;
using Amazon.S3;
using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.ContentService.Models;
using Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.ContentService.Services;

public class S3ContentResolver(IAmazonS3 s3) : IContentResolver<S3Item>
{
    public async Task<ContentItemData?> ResolveAsync(S3Item contentItem, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await s3.GetObjectAsync(contentItem.BucketName, contentItem.ObjectKey, cancellationToken);

            return new(response.ResponseStream,
                       response.Key,
                       contentItem.OverrideContentType ?? response.Headers.ContentType,
                       response.LastModified is not null
                           ? new(response.LastModified.Value)
                           : DateTimeOffset.UtcNow);
        }
        catch (AmazonS3Exception e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}
