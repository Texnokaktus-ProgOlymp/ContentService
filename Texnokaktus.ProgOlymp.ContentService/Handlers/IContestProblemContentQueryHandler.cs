using Texnokaktus.ProgOlymp.ContentService.Models;
using Texnokaktus.ProgOlymp.ContentService.Queries;

namespace Texnokaktus.ProgOlymp.ContentService.Handlers;

public interface
    IContestProblemContentQueryHandler : IQueryHandler<ContestProblemContentQuery, ContestProblemContentItems>;
