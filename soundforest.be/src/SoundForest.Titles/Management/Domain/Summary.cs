namespace SoundForest.Titles.Management.Domain;
public sealed record Summary(
    string? Id,
    string? Title,
    string? Type,
    Uri? Poster,
    int? StartYear,
    int? EndYear);
