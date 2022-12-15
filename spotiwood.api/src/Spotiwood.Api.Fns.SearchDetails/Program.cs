using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spotiwood.Api.SearchDetails;
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
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) =>
    {
        var uri = ctx.Configuration["SPOTIWOOD_OMDB_API"];
        var key = ctx.Configuration["SPOTIWOOD_OMDB_KEY"];

        services.AddMediatR(typeof(Program));

        services.AddSearchDetails(new Uri(uri), key);

        services.Configure<JsonSerializerOptions>(options =>
        {
            options.Converters.Add(new JsonStringEnumConverter());
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        });
    })
    .Build();

host.Run();

