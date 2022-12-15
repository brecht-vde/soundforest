using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Spotiwood.Framework.Application.Behaviors;

namespace Spotiwood.Framework;
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
