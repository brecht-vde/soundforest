using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SoundForest.Framework.Authentication;
using SoundForest.Playlists;
using SoundForest.Playlists.Application.Options;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

var host = new HostBuilder()
    .ConfigureAppConfiguration((ctx, cfg) =>
    {
        if (ctx.HostingEnvironment.IsDevelopment())
        {
            cfg.AddUserSecrets(Assembly.GetExecutingAssembly(), true).Build();
        }
    })
    .ConfigureFunctionsWorkerDefaults(app =>
    {
        app.UseFunctionAuthentication();
    })
    .ConfigureServices((ctx, services) =>
    {
        var audience = ctx.Configuration["SOUNDFOREST_AUTH_AUDIENCE"];
        var issuer = ctx.Configuration["SOUNDFOREST_AUTH_ISSUER"];

        services.AddFunctionAuthentication(audience, new Uri(issuer), Assembly.GetExecutingAssembly());

        var connectionString = ctx.Configuration["SOUNDFOREST_CONNECTIONSTRING"];
        var database = ctx.Configuration["SOUNDFOREST_DATABASE"];

        services.AddMediatR(typeof(Program));

        services.AddFeaturePlaylists(new ClientOptions(connectionString, database));

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
