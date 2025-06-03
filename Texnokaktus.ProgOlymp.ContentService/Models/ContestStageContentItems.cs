namespace Texnokaktus.ProgOlymp.ContentService.Models;

public record ContestStageContentItems(IEnumerable<ContentItemModel> Items,
                                       Dictionary<string, ContestProblemContentItems> ProblemContentItems);
