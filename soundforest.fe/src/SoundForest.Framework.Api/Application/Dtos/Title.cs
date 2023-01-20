namespace SoundForest.Framework.Api.Application.Dtos;
public sealed record TitleDetail(
    string? Id,
    string? Title,
    int? StartYear,
    int? EndYear,
    IEnumerable<string>? Genres,
    string? Plot,
    Uri? Poster,
    string? Type,
    int? Seasons,
    IEnumerable<string>? Actors);
