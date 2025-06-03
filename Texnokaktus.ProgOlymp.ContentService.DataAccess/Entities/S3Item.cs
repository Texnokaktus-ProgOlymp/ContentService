namespace Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;

public record S3Item(int Id,
                     string ContestName,
                     ContestStage? ContestStage,
                     string? ProblemAlias,
                     string ShortName,
                     string Description,
                     string? OverrideContentType,
                     string BucketName,
                     string ObjectKey) : ContentItem(Id,
                                                     ContestName,
                                                     ContestStage,
                                                     ProblemAlias,
                                                     ShortName,
                                                     Description,
                                                     OverrideContentType);
