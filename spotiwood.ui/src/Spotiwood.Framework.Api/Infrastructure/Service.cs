using OneOf;
using Spotiwood.Framework.Api.Application.Abstractions;
using Spotiwood.Framework.Api.Application.Dtos;
using Spotiwood.Framework.Api.Application.Serialization;
using System.Text.Json;

namespace Spotiwood.Framework.Api.Infrastructure;
internal sealed class Service : IService
{
    private readonly HttpClient _client;

    public Service(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task PlaylistDetailsAsync(string? identifier, CancellationToken cancellationToken = default)
    {
        try
        {

        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async Task PlaylistsAsync(int? page = null, int? size = null, CancellationToken cancellationToken = default)
    {
        try
        {

        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async Task<OneOf<Error, PagedCollection<SearchResult>>> SearchAsync(string? query, int? page = null, CancellationToken cancellationToken = default)
    {
        try
        {
            query = page is not null
                ? $"/api/search?q={query}&p={page}"
                : $"/api/search?q={query}";

            var request = new HttpRequestMessage(HttpMethod.Get, query);
            var response = await _client.SendAsync(request, cancellationToken);
            var stream = await response.Content.ReadAsStreamAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = await JsonSerializer.DeserializeAsync<Error>(stream, Serialization.DefaultOptions)
                    ?? throw new NullReferenceException();

                return error;
            }

            var result = await JsonSerializer.DeserializeAsync<PagedCollection<SearchResult>>(stream, Serialization.DefaultOptions)
                ?? throw new NullReferenceException();

            return result;

        }
        catch (Exception ex)
        {
            return new Error()
            {
                Message = "Whoops, something went wrong :(."
            };
        }
    }

    public async Task SearchDetailsAsync(string? identifier, CancellationToken cancellationToken = default)
    {
        try
        {

        }
        catch (Exception ex)
        {

            throw;
        }
    }
}
