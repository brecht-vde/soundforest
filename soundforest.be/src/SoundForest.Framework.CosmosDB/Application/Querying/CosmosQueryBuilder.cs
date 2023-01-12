// Credits: https://github.com/Azure/azure-cosmos-dotnet-v3/issues/893#issuecomment-1322292433
using Microsoft.Azure.Cosmos.Linq;
using SoundForest.Framework.CosmosDB.Application.Querying.Extensions;

namespace SoundForest.Framework.CosmosDB.Application.Querying;
internal sealed class CosmosQueryBuilder : ICosmosQueryBuilder
{
    public async Task<IList<T>> ToListAsync<T>(IQueryable<T> queryable)
        => await queryable.ToFeedIterator().ToAsyncEnumerable().ToListAsync();
}
