namespace Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;

public record YandexContestProblemTestItem(int Id,
                                           string ContestName,
                                           ContestStage? ContestStage,
                                           string? ProblemAlias,
                                           string ShortName,
                                           string Description,
                                           string OverrideFileName,
                                           string? OverrideContentType,
                                           string ProblemId) : ContentItem(Id,
                                                                             ContestName,
                                                                             ContestStage,
                                                                             ProblemAlias,
                                                                             ShortName,
                                                                             Description,
                                                                             OverrideFileName,
                                                                             OverrideContentType);
