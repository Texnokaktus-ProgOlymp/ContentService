namespace Texnokaktus.ProgOlymp.ContentService.Models;

public record ContentItemData(Stream Content, string FileName, string ContentType, DateTimeOffset LastModified);
