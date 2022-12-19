namespace Spotiwood.Api.Playlists.Infrastructure.Options;
internal sealed class DbOptions
{
    public string ConnectionString { get; init; } = default!;

    public string Database { get; init; } = "spotiwood";

    public string Container { get; init; } = "playlists";
}
