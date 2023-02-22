namespace SoundForest.Exports.Management.Domain;
public sealed record Export(string? Id, string? Name, string? Username, Status Status, string? ExternalId, string[]? Log);
