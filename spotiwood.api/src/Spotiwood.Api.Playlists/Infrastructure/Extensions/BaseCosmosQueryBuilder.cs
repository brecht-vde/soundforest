using Microsoft.Azure.Cosmos.Linq;

// Credits: https://github.com/Azure/azure-cosmos-dotnet-v3/issues/893#issuecomment-1322292433
namespace Spotiwood.Api.Playlists.Infrastructure.Extensions;
internal sealed class BaseCosmosQueryBuilder : ICosmosQueryBuilder
{
    public async Task<IList<T>> ToListAsync<T>(IQueryable<T> queryable)
        => await queryable.ToFeedIterator().ToAsyncEnumerable().ToListAsync();
}

