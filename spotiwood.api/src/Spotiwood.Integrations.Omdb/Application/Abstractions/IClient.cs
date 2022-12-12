using Spotiwood.Integrations.Omdb.Domain;

namespace Spotiwood.Integrations.Omdb.Application.Abstractions;

public interface IClient
{
    public Task<SearchResultCollection> SearchAsync(string query, int page, CancellationToken cancellationToken);
}
