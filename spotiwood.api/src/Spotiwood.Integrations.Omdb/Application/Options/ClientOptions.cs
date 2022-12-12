namespace Spotiwood.Integrations.Omdb.Application.Options;
public sealed record ClientOptions
{
    public Uri Uri { get; init; } = default!;

    public string Key { get; init; } = default!;
}
