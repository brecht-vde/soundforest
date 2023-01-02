using Microsoft.Extensions.DependencyInjection;

namespace Spotiwood.Framework.Messaging;
public static class DependencyInjection
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddScoped<IMessageBus, MessageBus>();
        return services;
    }
}
