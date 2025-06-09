namespace Texnokaktus.ProgOlymp.ContentService.Services.Abstractions;

public interface IContentLinkGenerator
{
    string? GetPath(string routeName, RouteValueDictionary pathValues);
}
