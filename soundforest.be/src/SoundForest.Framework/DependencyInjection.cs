using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SoundForest.Framework.Application.Behaviors;

namespace SoundForest.Framework;
public static class DependencyInjection
{
    public static IServiceCollection AddFramework(this IServiceCollection services)
    {
        // Mediatr
        services.AddMediatR(typeof(DependencyInjection));

        // Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResultRequestValidationBehavior<,>));

        return services;
    }
}
