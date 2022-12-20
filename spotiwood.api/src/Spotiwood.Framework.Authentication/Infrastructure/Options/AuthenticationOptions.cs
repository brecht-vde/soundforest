namespace Spotiwood.Framework.Authentication.Infrastructure.Options;
internal sealed record AuthenticationOptions
{
    public string? Audience { get; init; }

    public Uri? Issuer { get; init; }
}
