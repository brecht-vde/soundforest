using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SoundForest.Framework.Api;
using SoundForest.Framework.Messaging;
using SoundForest.App;
using SoundForest.Framework.Messaging.State;
using SoundForest.App.Features.Titles.States;
using SoundForest.App.Features.Playlists.Components.States;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddApi(builder.Configuration);
builder.Services.AddMessaging();
builder.Services.AddScoped<IStateContainer<QueryState?>, QueryStateContainer>();
builder.Services.AddScoped<IStateContainer<PlaylistQueryState?>, PlaylistQueryStateContainer>();


await builder.Build().RunAsync();
