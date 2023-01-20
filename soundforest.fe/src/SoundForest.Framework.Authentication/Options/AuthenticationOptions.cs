namespace SoundForest.Framework.Authentication.Options;

public sealed record AuthenticationOptions
{
    public string? Authority { get; init; }

    public string? Audience { get; init; }

    public string? ClientId { get; init; }

    public Uri? ApiRoot { get; init; }
}
