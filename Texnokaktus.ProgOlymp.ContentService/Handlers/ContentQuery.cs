using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.ContentService.Handlers;

public record ContentQuery(string ContestName, ContestStage? ContestStage, string? ProblemAlias, string ShortName);
