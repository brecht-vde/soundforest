namespace SoundForest.Framework.Application.Errors;
public sealed record Error(string? Message = null, Exception? Exception = null);
