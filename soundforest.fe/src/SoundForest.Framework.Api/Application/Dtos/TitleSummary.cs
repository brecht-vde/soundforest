namespace SoundForest.Framework.Api.Application.Dtos;
public sealed record TitleSummary(
    string? Id,
    string? Title,
    string? Type,
    Uri? Poster,
    int? StartYear,
    int? EndYear);
