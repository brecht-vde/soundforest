using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SoundForest.Exports;
using SoundForest.Playlists;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using CO_Exports = SoundForest.Exports.Application.Options.ClientOptions;
using CO_Playlists = SoundForest.Playlists.Application.Options.ClientOptions;

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

        services.AddMediatR(typeof(Program));

        services.AddFeatureExports(new CO_Exports(connectionString, database));
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

host.Run();
