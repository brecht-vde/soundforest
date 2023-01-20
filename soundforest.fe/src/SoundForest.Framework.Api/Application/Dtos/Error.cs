namespace SoundForest.Framework.Api.Application.Dtos;
public sealed record Error(string? Message, IEnumerable<string>? Errors = null);
