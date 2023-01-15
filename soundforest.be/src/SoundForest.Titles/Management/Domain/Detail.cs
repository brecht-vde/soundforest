namespace SoundForest.Titles.Management.Domain;
public sealed record Detail(
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
