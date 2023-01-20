using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SoundForest.Framework.Messaging;
public static class DependencyInjection
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddScoped<IMessageBus, MessageBus>();
        return services;
    }
}
