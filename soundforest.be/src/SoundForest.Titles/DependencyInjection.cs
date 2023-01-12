﻿using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SoundForest.Clients.Omdb;
using SoundForest.Clients.Omdb.Application.Options;
using SoundForest.Framework;
using SoundForest.Framework.Application.Pagination;
using SoundForest.Framework.Application.Requests;
using SoundForest.Titles.Application.Clients;
using SoundForest.Titles.Application.Queries;
using SoundForest.Titles.Application.Validators;
using SoundForest.Titles.Domain;
using SoundForest.Titles.Infrastructure.Clients;

namespace SoundForest.Titles;
public static class DependencyInjection
{
    public static IServiceCollection AddFeatureTitles(this IServiceCollection services, OmdbOptions options)
    {
        services.AddMemoryCache();
        services.AddLogging();
        services.AddMediatR(typeof(DependencyInjection));

        services.AddFramework();
        services.AddOmdb(options);

        services.AddTransient<IResultRequestHandler<SearchByIdQuery, Result<Detail>>, SearchByIdQueryHandler>();
        services.AddSingleton<IValidator<SearchByIdQuery>, SearchByIdQueryValidator>();

        services.AddTransient<IResultRequestHandler<FreeTextSearchQuery, Result<PagedCollection<Summary>>>, FreeTextSearchQueryHandler>();
        services.AddSingleton<IValidator<FreeTextSearchQuery>, FreeTextSearchQueryValidator>();

        services.AddTransient<IClient, TitleClient>();

        return services;
    }
}