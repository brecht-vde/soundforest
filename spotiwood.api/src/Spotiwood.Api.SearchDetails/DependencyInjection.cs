using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Spotiwood.Api.SearchDetails.Application.Queries;
using Spotiwood.Api.SearchDetails.Application.Validators;
using Spotiwood.Api.SearchDetails.Domain;
using Spotiwood.Framework;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.Integrations.Omdb;
using System.Reflection;

namespace Spotiwood.Api.SearchDetails;
public static class DependencyInjection
{
    public static IServiceCollection AddSearchDetails(this IServiceCollection services, Uri uri, string key)
    {
        // Mediatr
        services.AddMediatR(typeof(DependencyInjection));

        // Queries
        services.AddTransient<IResultRequestHandler<SearchByIdQuery, Result<SearchDetail>>, SearchByIdQueryHandler>();

        // Validators
        services.AddSingleton<IValidator<SearchByIdQuery>, SearchByIdQueryValidator>();

        // Mappers
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Add Omdb
        services.AddOmdb(uri, key);

        // Add Framework
        services.AddFramework();

        return services;
    }
}
