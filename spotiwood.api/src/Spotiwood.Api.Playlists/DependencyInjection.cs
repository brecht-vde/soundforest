using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Spotiwood.Api.Playlists.Application.Abstractions;
using Spotiwood.Api.Playlists.Application.Queries;
using Spotiwood.Api.Playlists.Application.Validators;
using Spotiwood.Api.Playlists.Domain;
using Spotiwood.Api.Playlists.Infrastructure.Clients;
using Spotiwood.Api.Playlists.Infrastructure.Extensions;
using Spotiwood.Api.Playlists.Infrastructure.Options;
using Spotiwood.Framework;
using Spotiwood.Framework.Application.Pagination;
using Spotiwood.Framework.Application.Requests;
using System.Reflection;

namespace Spotiwood.Api.Playlists;
public static class DependencyInjection
{
    public static IServiceCollection AddPlaylists(this IServiceCollection services, string connectionString)
    {
        // Mediatr
        services.AddMediatR(typeof(DependencyInjection));

        // Queries
        services.AddTransient<IResultRequestHandler<GetPlaylistByIdQuery, Result<Playlist>>, GetPlaylistByIdQueryHandler>();
        services.AddTransient<IResultRequestHandler<GetPlaylistsQuery, Result<PagedCollection<Playlist>>>, GetPlaylistsQueryHandler>();

        // Validators
        services.AddSingleton<IValidator<GetPlaylistByIdQuery>, GetPlaylistByIdQueryValidator>();
        services.AddSingleton<IValidator<GetPlaylistsQuery>, GetPlaylistsQueryValidator>();

        // Mappers
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Logging
        services.AddLogging();

        // Add Framework
        services.AddFramework();

        // Add options
        services.AddSingleton<IOptions<DbOptions>>(ctx =>
            Options.Create<DbOptions>(new DbOptions()
            {
                ConnectionString = connectionString
            })
        );

        // Add Cosmos DB
        services.AddSingleton<CosmosClient>(ctx =>
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") is "Development")
            {
                var cosmosClientOptions = new CosmosClientOptions()
                {
                    HttpClientFactory = () =>
                    {
                        var httpMessageHandler = new HttpClientHandler()
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };

                        return new HttpClient(httpMessageHandler);
                    },
                    ConnectionMode = ConnectionMode.Gateway
                };

                return new CosmosClient(connectionString, cosmosClientOptions);
            }

            return new CosmosClient(connectionString);
        });
        services.AddSingleton<ICosmosQueryBuilder, BaseCosmosQueryBuilder>();

        // DB Client
        services.AddTransient<IClient, BaseClient>();

        return services;
    }
}
