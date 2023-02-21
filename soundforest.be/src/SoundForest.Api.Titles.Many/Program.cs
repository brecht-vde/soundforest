using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SoundForest.Clients.Omdb.Search.Infrastructure.Options;
using SoundForest.Framework.Authentication;
using SoundForest.Titles;
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

        var uri = ctx.Configuration["SOUNDFOREST_OMDB_API"];
        var key = ctx.Configuration["SOUNDFOREST_OMDB_KEY"];

        services.AddMediatR(typeof(Program));

        services.AddFeatureTitles(new OmdbOptions(new Uri(uri), key));

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
