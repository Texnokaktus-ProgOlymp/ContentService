using System.Linq.Expressions;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Language.Flow;
using Texnokaktus.ProgOlymp.ContentService.DataAccess.Context;
using Texnokaktus.ProgOlymp.ContentService.DataAccess.Entities;
using Texnokaktus.ProgOlymp.ContentService.Handlers;
using Texnokaktus.ProgOlymp.ContentService.Models;
using Texnokaktus.ProgOlymp.ContentService.Queries;
using Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.ContentService.Tests;

[TestFixture]
public class ContentQueryHandlerTests
{
    private AppDbContext _appDbContext;
    private ContentQueryHandler _contentQueryHandler;
    private Mock<IContentLinkGenerator> _contentLinkGeneratorMock;

    [SetUp]
    public async Task Setup()
    {
        var dbContextOptions = new DbContextOptionsBuilder().UseInMemoryDatabase("test-db").Options;
        _appDbContext = new(dbContextOptions);
        await _appDbContext.Database.EnsureCreatedAsync();

        _contentLinkGeneratorMock = new();

        _contentQueryHandler = new(_appDbContext, _contentLinkGeneratorMock.Object);
    }

    [TearDown]
    public async Task Teardown()
    {
        await _appDbContext.Database.EnsureDeletedAsync();
        await _appDbContext.DisposeAsync();
    }
    
    [Test]
    public async Task ContestContentQueryHandlerTest_Hierarchy_Empty()
    {
        var expected = new ContestContentItems([], new());

        var contentItems = await _contentQueryHandler.HandleAsync(new ContestContentQuery("test"));

        Assert.That(contentItems, Is.EqualTo(expected).UsingPropertiesComparer());

        _contentLinkGeneratorMock.VerifyNoOtherCalls();
    }
    
    [Test]
    public async Task ContestContentQueryHandlerTest_Hierarchy_ContestRoot()
    {
        _appDbContext.ContentItems.AddRange(new GitHubReleaseItem(0,
                                                                  "test",
                                                                  null,
                                                                  null,
                                                                  "item1",
                                                                  "Description 1",
                                                                  null,
                                                                  null,
                                                                  "owner",
                                                                  "repo",
                                                                  "asset1.bin"),
                                            new GitHubReleaseItem(0,
                                                                  "test",
                                                                  null,
                                                                  null,
                                                                  "item2",
                                                                  "Description 2",
                                                                  null,
                                                                  null,
                                                                  "owner",
                                                                  "repo",
                                                                  "asset2.bin"));
        await _appDbContext.SaveChangesAsync();

        _contentLinkGeneratorMock.SetupContestContentItem("test", "item1").Returns("/link1");
        _contentLinkGeneratorMock.SetupContestContentItem("test", "item2").Returns("/link2");

        var expected = new ContestContentItems([
                                                   new("Description 1", "/link1"),
                                                   new("Description 2", "/link2")
                                               ],
                                               new());

        var contentItems = await _contentQueryHandler.HandleAsync(new ContestContentQuery("test"));

        Assert.That(contentItems, Is.EqualTo(expected).UsingPropertiesComparer());
        _contentLinkGeneratorMock.VerifyContestContentItem("test", "item1", Times.Once());
        _contentLinkGeneratorMock.VerifyContestContentItem("test", "item2", Times.Once());
        _contentLinkGeneratorMock.VerifyNoOtherCalls();
    }

    [Test]
    public async Task ContestContentQueryHandlerTest()
    {
        _appDbContext.ContentItems.AddRange(new GitHubReleaseItem(0,
                                                                  "test",
                                                                  ContestStage.Preliminary,
                                                                  null,
                                                                  "item",
                                                                  "Description 1",
                                                                  null,
                                                                  null,
                                                                  "owner",
                                                                  "repo",
                                                                  "asset1.bin"),
                                            new GitHubReleaseItem(0,
                                                                  "test",
                                                                  ContestStage.Final,
                                                                  null,
                                                                  "item",
                                                                  "Description 2",
                                                                  null,
                                                                  null,
                                                                  "owner",
                                                                  "repo",
                                                                  "asset2.bin"));
        await _appDbContext.SaveChangesAsync();

        _contentLinkGeneratorMock.SetupContestStageContentItem("test", ContestStage.Preliminary, "item").Returns("/link1");
        _contentLinkGeneratorMock.SetupContestStageContentItem("test", ContestStage.Final, "item").Returns("/link2");

        var expected = new ContestContentItems([],
                                               new()
                                               {
                                                   [ContestStage.Preliminary] = new([
                                                                                        new("Description 1", "/link1")
                                                                                    ],
                                                                                    new()),
                                                   [ContestStage.Final] = new([
                                                                                  new("Description 2", "/link2")
                                                                              ],
                                                                              new())
                                               });

        var contentItems = await _contentQueryHandler.HandleAsync(new ContestContentQuery("test"));

        Assert.That(contentItems, Is.EqualTo(expected).UsingPropertiesComparer());

        _contentLinkGeneratorMock.VerifyContestStageContentItem("test", ContestStage.Preliminary, "item", Times.Once());
        _contentLinkGeneratorMock.VerifyContestStageContentItem("test", ContestStage.Final, "item", Times.Once());
        _contentLinkGeneratorMock.VerifyNoOtherCalls();
    }

    [TestCaseSource(nameof(TestCases))]
    public async Task ContestContentQueryHandlerTest_Multi((ContentItem Item, Action<Mock<IContentLinkGenerator>> SetupAction, Action<Mock<IContentLinkGenerator>> VerifyAction)[] items, ContestContentItems expected)
    {
        _appDbContext.ContentItems.AddRange(items.Select(x => x.Item));
        await _appDbContext.SaveChangesAsync();
        
        foreach (var action in items.Select(x => x.SetupAction)) action.Invoke(_contentLinkGeneratorMock);
        
        var contentItems = await _contentQueryHandler.HandleAsync(new ContestContentQuery("test"));
        Assert.That(contentItems, Is.EqualTo(expected).UsingPropertiesComparer());
        
        
        foreach (var action in items.Select(x => x.VerifyAction)) action.Invoke(_contentLinkGeneratorMock);
        _contentLinkGeneratorMock.VerifyNoOtherCalls();
    }

    private static IEnumerable<TestCaseData> TestCases()
    {
        yield return new TestCaseData(Array.Empty<(ContentItem Item, Action<Mock<IContentLinkGenerator>> SetupAction, Action<Mock<IContentLinkGenerator>> VerifyAction)>(),
                                      new ContestContentItems([], new())).SetName("Empty");

        yield return new TestCaseData(new (ContentItem Item, Action<Mock<IContentLinkGenerator>> SetupAction, Action<Mock<IContentLinkGenerator>> VerifyAction)[]
                                      {
                                          (new GitHubReleaseItem(0,
                                                                 "test",
                                                                 ContestStage.Preliminary,
                                                                 null,
                                                                 "item",
                                                                 "Description 1",
                                                                 null,
                                                                 null,
                                                                 "owner",
                                                                 "repo",
                                                                 "asset1.bin"),
                                           mock => mock.SetupContestStageContentItem("test", ContestStage.Preliminary, "item")
                                                       .Returns("/link1"),
                                           mock => mock.VerifyContestStageContentItem("test", ContestStage.Preliminary, "item", Times.Once())),
                                          (new GitHubReleaseItem(0,
                                                                 "test",
                                                                 ContestStage.Final,
                                                                 null,
                                                                 "item",
                                                                 "Description 2",
                                                                 null,
                                                                 null,
                                                                 "owner",
                                                                 "repo",
                                                                 "asset2.bin"),
                                           mock => mock.SetupContestStageContentItem("test", ContestStage.Final, "item")
                                                       .Returns("/link2"),
                                           mock => mock.VerifyContestStageContentItem("test", ContestStage.Final, "item", Times.Once())),
                                      },
                                      new ContestContentItems([],
                                                              new()
                                                              {
                                                                  [ContestStage.Preliminary] = new([
                                                                                                       new("Description 1",
                                                                                                           "/link1")
                                                                                                   ],
                                                                                                   new()),
                                                                  [ContestStage.Final] = new([
                                                                                                 new("Description 2",
                                                                                                     "/link2")
                                                                                             ],
                                                                                             new())
                                                              })).SetName("Two items in separate contest stages");
    }
}

file static class ContentLinkGeneratorMockExtensions
{
    private const string ContestRouteName = "GetContestContentItem";
    private const string ContestStageRouteName = "GetContestStageContentItem";
    private const string ContestProblemRouteName = "GetContestProblemContentItem";

    public static ISetup<IContentLinkGenerator, string?> SetupContestContentItem(this Mock<IContentLinkGenerator> mock, string contestName, string shortName) =>
        mock.SetupContentItem(ContestRouteName, contestName, null, null, shortName);

    public static void VerifyContestContentItem(this Mock<IContentLinkGenerator> mock, string contestName, string shortName, Times times) =>
        mock.VerifyContentItem(ContestRouteName, contestName, null, null, shortName, times);

    public static ISetup<IContentLinkGenerator, string?> SetupContestStageContentItem(this Mock<IContentLinkGenerator> mock, string contestName, ContestStage contestStage, string shortName) =>
        mock.SetupContentItem(ContestStageRouteName, contestName, contestStage, null, shortName);

    public static void VerifyContestStageContentItem(this Mock<IContentLinkGenerator> mock, string contestName, ContestStage contestStage, string shortName, Times times) =>
        mock.VerifyContentItem(ContestStageRouteName, contestName, contestStage, null, shortName, times);

    public static ISetup<IContentLinkGenerator, string?> SetupContestProblemContentItem(this Mock<IContentLinkGenerator> mock, string contestName, ContestStage contestStage, string problemAlias, string shortName) =>
        mock.SetupContentItem(ContestProblemRouteName, contestName, contestStage, problemAlias, shortName);

    public static void VerifyContestProblemContentItem(this Mock<IContentLinkGenerator> mock, string contestName, ContestStage contestStage, string problemAlias, string shortName, Times times) =>
        mock.VerifyContentItem(ContestProblemRouteName, contestName, contestStage, problemAlias, shortName, times);

    private static ISetup<IContentLinkGenerator, string?> SetupContentItem(this Mock<IContentLinkGenerator> mock, string routeName, string contestName, ContestStage? contestStage, string? problemAlias, string shortName) =>
        mock.Setup(generator => generator.GetPath(routeName,
                                                  It.Is(GetExpression(contestName, contestStage, problemAlias, shortName))));

    private static void VerifyContentItem(this Mock<IContentLinkGenerator> mock, string routeName, string contestName, ContestStage? contestStage, string? problemAlias, string shortName, Times times) =>
        mock.Verify(generator => generator.GetPath(routeName,
                                                   It.Is(GetExpression(contestName, contestStage, problemAlias, shortName))),
                    times);

    private static Expression<Func<RouteValueDictionary, bool>> GetExpression(string contestName,
                                                                              ContestStage? contestStage,
                                                                              string? problemAlias,
                                                                              string shortName) =>
        routeValues => routeValues["contestName"]!.Equals(contestName)
                    && (!contestStage.HasValue || routeValues["contestStage"]!.Equals(contestStage))
                    && (problemAlias == null || routeValues["problemAlias"]!.Equals(problemAlias))
                    && routeValues["shortName"]!.Equals(shortName);
}
