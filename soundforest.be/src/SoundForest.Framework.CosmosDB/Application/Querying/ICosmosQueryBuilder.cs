// Credits: https://github.com/Azure/azure-cosmos-dotnet-v3/issues/893#issuecomment-1322292433

namespace SoundForest.Framework.CosmosDB.Application.Querying;
public interface ICosmosQueryBuilder
{
    public Task<IList<T>> ToListAsync<T>(IQueryable<T> queryable);
}
