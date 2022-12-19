// Credits: https://github.com/Azure/azure-cosmos-dotnet-v3/issues/893#issuecomment-1322292433
namespace Spotiwood.Api.Playlists.Infrastructure.Extensions;
internal interface ICosmosQueryBuilder
{
    public Task<IList<T>> ToListAsync<T>(IQueryable<T> queryable);
}