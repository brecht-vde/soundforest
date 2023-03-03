using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SoundForest.Clients.Auth0.Authentication.Infrastructure.Options;
using SoundForest.Clients.Spotify.Authentication.Infrastructure.Options;
using SoundForest.Exports;
using SoundForest.Exports.Processing.Infrastructure.Stores;
using SoundForest.Playlists;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using CO_Exports = SoundForest.Exports.Management.Infrastructure.Options.ClientOptions;
using CO_Playlists = SoundForest.Playlists.Management.Application.Options.ClientOptions;

var host = new HostBuilder()
    .ConfigureAppConfiguration((ctx, cfg) =>
    {
        if (ctx.HostingEnvironment.IsDevelopment())
        {
            cfg.AddUserSecrets(Assembly.GetExecutingAssembly(), true).Build();
        }
    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) =>
    {
        var connectionString = ctx.Configuration["SOUNDFOREST_CONNECTIONSTRING"];
        var database = ctx.Configuration["SOUNDFOREST_DATABASE"];
        var tsvFileUri = ctx.Configuration["SOUNDFOREST_TSV_FILEURI"];
        var spotifyBaseUri = ctx.Configuration["SOUNDFOREST_SPOTIFY_BASEURI"];
        var spotifyClientId = ctx.Configuration["SOUNDFOREST_SPOTIFY_CLIENTID"];
        var spotifyClientSecret = ctx.Configuration["SOUNDFOREST_SPOTIFY_CLIENTSECRET"];
        var auth0MgmtBaseUri = ctx.Configuration["SOUNDFOREST_AUTH0MGMT_BASEURI"];
        var auth0MgmtClientId = ctx.Configuration["SOUNDFOREST_AUTH0MGMT_CLIENTID"];
        var auth0MgmtClientSecret = ctx.Configuration["SOUNDFOREST_AUTH0MGMT_CLIENTSECRET"];
        var auth0Audience = ctx.Configuration["SOUNDFOREST_AUTH0MGMT_AUDIENCE"];

        services.AddMediatR(typeof(Program));

        services.AddFeatureExportManagement(new CO_Exports(connectionString, database));

        services.AddFeatureExportProcessing(
            new SpotifyAuthOptions(spotifyClientId, spotifyClientSecret, new Uri(spotifyBaseUri)),
            new Auth0Options(auth0MgmtClientId, auth0MgmtClientSecret, auth0Audience, new Uri(auth0MgmtBaseUri)),
            new TsvOptions(new Uri(tsvFileUri)));

        services.AddFeaturePlaylists(new CO_Playlists(connectionString, database));

        services.Configure<JsonSerializerOptions>(options =>
        {
            options.Converters.Add(new JsonStringEnumConverter());
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.PropertyNameCaseInsensitive = true;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        });
    })
    .Build();

//await host.PreloadDataAsync();
await host.RunAsync();
