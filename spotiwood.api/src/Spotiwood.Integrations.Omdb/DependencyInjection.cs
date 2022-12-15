using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Spotiwood.Integrations.Omdb.Application.Abstractions;
using Spotiwood.Integrations.Omdb.Application.Clients;
using Spotiwood.Integrations.Omdb.Application.Options;
using Spotiwood.Integrations.Omdb.Infrastructure.Clients;
using System.Reflection;

namespace Spotiwood.Integrations.Omdb;
public static class DependencyInjection
{
    public static IServiceCollection AddOmdb(this IServiceCollection services, Uri uri, string key)
        => AddOmdb(services, new ClientOptions() { Uri = uri, Key = key });

    public static IServiceCollection AddOmdb(this IServiceCollection services, ClientOptions options)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        if (options?.Uri is null)
            throw new ArgumentNullException("A uri must be provided.");

        if (string.IsNullOrWhiteSpace(options?.Key))
            throw new ArgumentNullException("A key must be provided.");

        services.AddSingleton<IOptions<ClientOptions>>(ctx => Options.Create(options));

        services.AddTransient<IClient, BaseClient>();

        services.AddHttpClient<IDbClient, DbClient>(ctx =>
        {
            ctx.BaseAddress = options.Uri;
        });

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddLogging();

        return services;
    }
}
