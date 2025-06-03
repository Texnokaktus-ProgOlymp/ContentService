using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;

namespace Texnokaktus.ProgOlymp.ContentService.Models;

public record ContestContentItems(IEnumerable<ContentItemModel> Items,
                                  Dictionary<ContestStage, ContestStageContentItems> ContestStageContentItems);
