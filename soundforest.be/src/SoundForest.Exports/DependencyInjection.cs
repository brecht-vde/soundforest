﻿using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SoundForest.Clients.Auth0.Authentication;
using SoundForest.Clients.Auth0.Authentication.Infrastructure.Options;
using SoundForest.Clients.Spotify.Authentication;
using SoundForest.Clients.Spotify.Authentication.Infrastructure.Options;
using SoundForest.Exports.Management.Application.Clients;
using SoundForest.Exports.Management.Application.Commands;
using SoundForest.Exports.Management.Application.Queries;
using SoundForest.Exports.Management.Application.Validators;
using SoundForest.Exports.Management.Domain;
using SoundForest.Exports.Management.Infrastructure.Clients;
using SoundForest.Exports.Management.Infrastructure.Options;
using SoundForest.Exports.Processing.Application.Commands;
using SoundForest.Exports.Processing.Application.Exporters;
using SoundForest.Exports.Processing.Application.Parsers;
using SoundForest.Exports.Processing.Application.Stores;
using SoundForest.Exports.Processing.Application.Validators;
using SoundForest.Exports.Processing.Domain;
using SoundForest.Exports.Processing.Infrastructure.Exporters;
using SoundForest.Exports.Processing.Infrastructure.Exporters.Factories;
using SoundForest.Exports.Processing.Infrastructure.Exporters.Services;
using SoundForest.Exports.Processing.Infrastructure.Parsers;
using SoundForest.Exports.Processing.Infrastructure.Stores;
using SoundForest.Framework;
using SoundForest.Framework.Application.Requests;
using SoundForest.Framework.CosmosDB;

namespace SoundForest.Exports;
public static class DependencyInjection
{
    public static IServiceCollection AddFeatureExportManagement(this IServiceCollection services, ClientOptions options)
    {
        services.AddLogging();
        services.AddMediatR(typeof(DependencyInjection));
        services.TryAddSingleton<CosmosClient>(ctx => new CosmosClient(options?.ConnectionString));

        services.AddCosmosDBExtensions();
        services.AddFramework();

        services.TryAddTransient<IResultRequestHandler<CreateExportCommand, Result<Export>>, CreateExportCommandHandler>();
        services.TryAddTransient<IResultRequestHandler<ExportByIdQuery, Result<Export>>, ExportByIdQueryHandler>();
        services.TryAddTransient<IResultRequestHandler<UpsertExportCommand, Result<Export>>, UpsertExportCommandHandler>();

        services.TryAddSingleton<IValidator<CreateExportCommand>, CreateExportCommandValidator>();
        services.TryAddSingleton<IValidator<ExportByIdQuery>, ExportByIdQueryValidator>();
        services.TryAddSingleton<IValidator<UpsertExportCommand>, UpsertExportCommandValidator>();

        services.TryAddSingleton<IOptions<ClientOptions>>(ctx => Options.Create<ClientOptions>(options));
        services.TryAddTransient<IClient, ExportClient>();

        return services;
    }

    public static IServiceCollection AddFeatureExportProcessing(this IServiceCollection services, SpotifyAuthOptions spotify, Auth0Options auth0, TsvOptions tsv)
    {
        services.AddLogging();
        services.AddMediatR(typeof(DependencyInjection));
        services.AddMemoryCache();

        services.AddFramework();
        services.AddSpotifyAuth(spotify);
        services.AddAuth0(auth0);

        services.TryAddTransient<IResultRequestHandler<ProcessExportCommand, Result<ProcessExportCommandResponse>>, ProcessExportCommandHandler>();
        services.TryAddSingleton<IValidator<ProcessExportCommand>, ProcessExportCommandValidator>();

        services.TryAddSingleton<IOptions<TsvOptions>>(ctx => Options.Create<TsvOptions>(tsv));

        services.AddSingleton<IKeyValueStore<IEnumerable<string>?>, TsvKeyValueStore>();
        services.AddTransient<IParsingService<IEnumerable<Soundtrack>?>, SoundtrackParser>();
        services.AddTransient<IExporter<IEnumerable<Soundtrack>?>, SpotifyExporter>();
        services.AddTransient<ISpotifyClientFactory, SpotifyClientFactory>();
        services.AddTransient<IDocumentLoader, ImdbDocumentLoader>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<ISearchService, SearchService>();
        services.AddTransient<IPlaylistService, PlaylistService>();

        services.AddHttpClient();

        return services;
    }
}
