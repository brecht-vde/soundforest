﻿namespace SoundForest.Clients.Omdb.Domain;
public sealed record SearchSummary(
    string? Id,
    string? Title,
    int? StartYear,
    int? EndYear,
    string? Type,
    Uri? Poster);
