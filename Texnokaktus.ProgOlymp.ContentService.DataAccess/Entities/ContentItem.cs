namespace Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;

public abstract record ContentItem(int Id,
                                   string ContestName,
                                   ContestStage? ContestStage,
                                   string? ProblemAlias,
                                   string ShortName,
                                   string Description,
                                   string? OverrideFileName,
                                   string? OverrideContentType);
