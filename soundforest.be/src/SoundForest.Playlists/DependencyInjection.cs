using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoundForest.Framework;
using SoundForest.Framework.Application.Pagination;
using SoundForest.Framework.Application.Requests;
using SoundForest.Framework.CosmosDB;
using SoundForest.Playlists.Management.Application.Clients;
using SoundForest.Playlists.Management.Application.Commands;
using SoundForest.Playlists.Management.Application.Options;
using SoundForest.Playlists.Management.Application.Queries;
using SoundForest.Playlists.Management.Application.Validators;
using SoundForest.Playlists.Management.Domain;
using SoundForest.Playlists.Management.Infrastructure.Clients;

namespace SoundForest.Playlists;
public static class DependencyInjection
{
    public static IServiceCollection AddFeaturePlaylists(this IServiceCollection services, ClientOptions options)
    {
        services.AddMediatR(typeof(DependencyInjection));

        services.AddTransient<IResultRequestHandler<PlaylistByIdQuery, Result<Playlist>>, PlaylistByIdQueryHandler>();
        services.AddTransient<IResultRequestHandler<PlaylistsQuery, Result<PagedCollection<Playlist>>>, PlaylistsQueryHandler>();
        services.AddTransient<IResultRequestHandler<CreatePlaylistCommand, Result<Playlist>>, CreatePlaylistCommandHandler>();

        services.AddSingleton<IValidator<PlaylistByIdQuery>, PlaylistByIdQueryValidator>();
        services.AddSingleton<IValidator<PlaylistsQuery>, PlaylistsQueryValidator>();
        services.AddSingleton<IValidator<CreatePlaylistCommand>, CreatePlaylistCommandValidator>();

        services.AddLogging();

        services.AddCosmosDBExtensions();
        services.AddFramework();

        services.AddSingleton(ctx => Options.Create(options));

        services.AddSingleton(ctx => new CosmosClient(options?.ConnectionString));

        services.AddTransient<IClient, PlaylistClient>();

        return services;
    }
}
