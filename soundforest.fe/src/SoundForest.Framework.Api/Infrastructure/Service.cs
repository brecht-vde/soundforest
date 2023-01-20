using OneOf;
using SoundForest.Framework.Api.Application.Abstractions;
using SoundForest.Framework.Api.Application.Dtos;
using SoundForest.Framework.Api.Application.Serialization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace SoundForest.Framework.Api.Infrastructure;
internal sealed class Service : IService
{
    private readonly HttpClient _client;

    public Service(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<OneOf<Error, Export>> CreateExportAsync(string? identifier, string? title, CancellationToken cancellationToken = default)
        => await PostRequestAsync<Export>(
            uri: new Uri("/api/exports", UriKind.Relative),
            content: JsonContent.Create(new
            {
                Id = identifier,
                Name = title
            }),
            cancellationToken: cancellationToken);

    public async Task<OneOf<Error, Export>> ExportAsync(string? identifier, CancellationToken cancellationToken = default)
        => await GetRequestAsync<Export>(
            uri: new Uri($"/api/exports/{identifier}", UriKind.Relative),
            cancellationToken: cancellationToken);

    public async Task<OneOf<Error, Playlist>> PlaylistAsync(string? identifier, CancellationToken cancellationToken = default)
        => await GetRequestAsync<Playlist>(
            uri: new Uri($"/api/playlists/{identifier}", UriKind.Relative),
            cancellationToken: cancellationToken);

    public async Task<OneOf<Error, PagedCollection<Playlist>>> PlaylistsAsync(int? page = null, int? size = null, CancellationToken cancellationToken = default)
        => await GetRequestAsync<PagedCollection<Playlist>>(
            uri: PlaylistsUri(page, size),
            cancellationToken: cancellationToken);

    public async Task<OneOf<Error, TitleDetail>> TitleAsync(string? identifier, CancellationToken cancellationToken = default)
        => await GetRequestAsync<TitleDetail>(
            uri: new Uri($"/api/titles/{identifier}", UriKind.Relative),
            cancellationToken: cancellationToken);

    public async Task<OneOf<Error, PagedCollection<TitleSummary>>> TitlesAsync(string? query, int? page = null, CancellationToken cancellationToken = default)
        => await GetRequestAsync<PagedCollection<TitleSummary>>(
            uri: TitlesUri(query, page),
            cancellationToken: cancellationToken);

    private Func<string?, int?, Uri?> TitlesUri = (string? query, int? page)
        => page is not null
            ? new Uri($"/api/titles?q={query}&p={page}", UriKind.Relative)
            : new Uri($"/api/titles?q={query}", UriKind.Relative);

    private Func<int?, int?, Uri?> PlaylistsUri = (int? page, int? size)
        =>
        {
            var query = HttpUtility.ParseQueryString("");

            if (page is not null)
                query["page"] = $"{page}";

            if (size is not null)
                query["size"] = $"{size}";

            return new Uri($"/api/playlists?{query}", UriKind.Relative);
        };

    private async Task<OneOf<Error, T>> GetRequestAsync<T>(Uri? uri, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await _client.SendAsync(request, cancellationToken);
            var stream = await response.Content.ReadAsStreamAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = await JsonSerializer.DeserializeAsync<Error>(stream, Serialization.DefaultOptions)
                    ?? throw new NullReferenceException();

                return error;
            }

            var result = await JsonSerializer.DeserializeAsync<T>(stream, Serialization.DefaultOptions)
                ?? throw new NullReferenceException();

            return result;
        }
        catch (Exception ex)
        {
            return new Error("Whoops, something went wrong :(.");
        }
    }

    private async Task<OneOf<Error, T>> PostRequestAsync<T>(Uri? uri, JsonContent? content, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = content;

            var response = await _client.SendAsync(request, cancellationToken);
            var a = await response.Content.ReadAsStringAsync();
            var stream = await response.Content.ReadAsStreamAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = await JsonSerializer.DeserializeAsync<Error>(stream, Serialization.DefaultOptions)
                    ?? throw new NullReferenceException();

                return error;
            }

            var result = await JsonSerializer.DeserializeAsync<T>(stream, Serialization.DefaultOptions)
                ?? throw new NullReferenceException();

            return result;
        }
        catch (Exception ex)
        {
            return new Error("Whoops, something went wrong :(.");
        }
    }
}
