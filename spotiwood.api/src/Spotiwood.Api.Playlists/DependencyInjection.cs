using Azure.Data.Tables;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Spotiwood.Api.Playlists.Application.Abstractions;
using Spotiwood.Api.Playlists.Application.Queries;
using Spotiwood.Api.Playlists.Application.Validators;
using Spotiwood.Api.Playlists.Domain;
using Spotiwood.Api.Playlists.Infrastructure.Clients;
using Spotiwood.Framework;
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

        // Validators
        services.AddSingleton<IValidator<GetPlaylistByIdQuery>, GetPlaylistByIdQueryValidator>();

        // Mappers
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Logging
        services.AddLogging();

        // Add Framework
        services.AddFramework();

        // Add tableclient
        services.AddSingleton<TableClient>(ctx =>
        {
            var options = new TableClientOptions();
            options.Retry.Mode = Azure.Core.RetryMode.Exponential;
            options.Retry.MaxDelay = TimeSpan.FromSeconds(600);
            options.Retry.MaxRetries = 4;
            options.Retry.Delay = TimeSpan.FromSeconds(3);
            options.Retry.NetworkTimeout = TimeSpan.FromSeconds(600);

            return new TableClient(connectionString, "Playlists", options);
        });

        // DB Client
        services.AddTransient<IClient, BaseClient>();

        return services;
    }
}
