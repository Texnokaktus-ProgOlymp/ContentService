using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.ContentService.Queries;

public record ContentQuery(string ContestName, ContestStage? ContestStage, string? ProblemAlias, string ShortName);
