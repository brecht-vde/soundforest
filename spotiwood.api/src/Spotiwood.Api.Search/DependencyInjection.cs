using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Spotiwood.Api.Search.Application.Queries;
using Spotiwood.Api.Search.Application.Validators;
using Spotiwood.Api.Search.Domain;
using Spotiwood.Framework;
using Spotiwood.Framework.Application.Pagination;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.Integrations.Omdb;
using System.Reflection;

namespace Spotiwood.Api.Search;
public static class DependencyInjection
{
    public static IServiceCollection AddSearch(this IServiceCollection services, Uri uri, string key)
    {
        // Mediatr
        services.AddMediatR(typeof(DependencyInjection));

        // Queries
        services.AddTransient<IResultRequestHandler<FreeTextSearchQuery, Result<PagedCollection<SearchResult>>>, FreeTextSearchQueryHandler>();

        // Validators
        services.AddSingleton<IValidator<FreeTextSearchQuery>, FreeTextSearchQueryValidator>();

        // Mappers
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Logging
        services.AddLogging();

        // Add Omdb
        services.AddOmdb(uri, key);

        // Add Framework
        services.AddFramework();

        return services;
    }
}
