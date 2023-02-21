namespace SoundForest.Clients.Omdb.Search.Domain;
public sealed record SearchSummary(
    string? Id,
    string? Title,
    int? StartYear,
    int? EndYear,
    string? Type,
    Uri? Poster);
