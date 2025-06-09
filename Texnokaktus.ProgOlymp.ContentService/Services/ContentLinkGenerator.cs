using Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.ContentService.Services;

internal class ContentLinkGenerator(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator) : IContentLinkGenerator
{
    public string? GetPath(string routeName, RouteValueDictionary pathValues) =>
        httpContextAccessor.HttpContext is { } httpContext
            ? linkGenerator.GetPathByRouteValues(httpContext, routeName, pathValues)
            : null;
}
