namespace SoundForest.Titles.Domain;
public sealed record Summary(
    string? Id,
    string? Title,
    string? Type,
    Uri? Poster,
    int? StartYear,
    int? EndYear);
