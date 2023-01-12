// Credits: https://github.com/Azure/azure-cosmos-dotnet-v3/issues/893#issuecomment-1322292433
using Microsoft.Azure.Cosmos;

namespace SoundForest.Framework.CosmosDB.Application.Querying.Extensions;
internal static class FeedIteratorExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this FeedIterator<T> iterator)
    {
        while (iterator.HasMoreResults)
        {
            foreach (var item in await iterator.ReadNextAsync())
            {
                yield return item;
            }
        }
    }
}