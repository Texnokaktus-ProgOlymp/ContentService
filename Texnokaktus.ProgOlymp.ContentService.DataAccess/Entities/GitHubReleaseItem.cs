namespace Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;

public record GitHubReleaseItem(int Id,
                                string ContestName,
                                ContestStage? ContestStage,
                                string? ProblemAlias,
                                string ShortName,
                                string Description,
                                string? OverrideContentType,
                                string OwnerName,
                                string RepositoryName,
                                string AssetName) : ContentItem(Id,
                                                                ContestName,
                                                                ContestStage,
                                                                ProblemAlias,
                                                                ShortName,
                                                                Description,
                                                                OverrideContentType);
