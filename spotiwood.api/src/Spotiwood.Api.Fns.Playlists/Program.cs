using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spotiwood.Api.Playlists;
using Spotiwood.Framework.Authentication;
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
        var audience = ctx.Configuration["SPOTIWOOD_AUTH_AUDIENCE"];
        var issuer = ctx.Configuration["SPOTIWOOD_AUTH_ISSUER"];

        services.AddFunctionAuthentication(audience, new Uri(issuer), Assembly.GetExecutingAssembly());

        var connectionString = ctx.Configuration["SPOTIWOOD_CONNECTIONSTRING"];

        services.AddMediatR(typeof(Program));

        services.AddPlaylists(connectionString);

        services.Configure<JsonSerializerOptions>(options =>
        {
            options.Converters.Add(new JsonStringEnumConverter());
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        });

    })
    .Build();

host.Run();
