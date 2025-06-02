using Amazon.S3;
using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.ContentService.Models;
using Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.ContentService.Services;

public class S3ContentResolver(IAmazonS3 s3) : IContentResolver<S3Item>
{
    public async Task<ContentItemData?> ResolveAsync(S3Item contentItem)
    {
        var response = await s3.GetObjectAsync(contentItem.BucketName, contentItem.ObjectKey);

        return new(response.ResponseStream,
                   response.Key,
                   contentItem.OverrideContentType ?? response.Headers.ContentType,
                   response.LastModified is not null
                       ? new(response.LastModified.Value)
                       : DateTimeOffset.UtcNow);
    }
}
