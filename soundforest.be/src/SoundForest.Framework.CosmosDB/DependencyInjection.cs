using Microsoft.Extensions.DependencyInjection;
using SoundForest.Framework.CosmosDB.Application.Querying;

namespace SoundForest.Framework.CosmosDB;
public static class DependencyInjection
{
    public static IServiceCollection AddCosmosDBExtensions(this IServiceCollection services)
    {
        services.AddSingleton<ICosmosQueryBuilder, CosmosQueryBuilder>();
        return services;
    }
}
