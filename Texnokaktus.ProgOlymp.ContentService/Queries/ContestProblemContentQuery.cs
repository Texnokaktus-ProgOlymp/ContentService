using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.ContentService.Queries;

public record ContestProblemContentQuery(string ContestName, ContestStage ContestStage, string ProblemAlias);
