using Azure;
using Azure.Data.Tables;

namespace Spotiwood.Api.Playlists.Infrastructure.Entities;
internal sealed class PlaylistEntity : ITableEntity
{
    // Identifier
    public string PartitionKey { get; set; } = default!;

    // Identifier
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; } = default!;

    public string? Title { get; set; }
    public string? PlaylistTitle { get; set; }
    public string? PlaylistUri { get; set; }
}
