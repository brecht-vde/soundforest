using Spotiwood.Integrations.Omdb.Application.Dtos;

namespace Spotiwood.Integrations.Omdb.Application.Abstractions;
internal interface IDbClient
{
    public Task<SearchResultDtoCollection> SearchAsync(string query, int page, CancellationToken cancellationToken);
}
