using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoundForest.Exports.Application.Clients;
using SoundForest.Exports.Application.Commands;
using SoundForest.Exports.Application.Options;
using SoundForest.Exports.Application.Queries;
using SoundForest.Exports.Application.Validators;
using SoundForest.Exports.Domain;
using SoundForest.Exports.Infrastructure.Clients;
using SoundForest.Framework;
using SoundForest.Framework.Application.Requests;
using SoundForest.Framework.CosmosDB;

namespace SoundForest.Exports;
public static class DependencyInjection
{
    public static IServiceCollection AddFeatureExports(this IServiceCollection services, ClientOptions options)
    {
        services.AddLogging();
        services.AddMediatR(typeof(DependencyInjection));
        services.AddSingleton<CosmosClient>(ctx => new CosmosClient(options?.ConnectionString));

        services.AddCosmosDBExtensions();
        services.AddFramework();

        services.AddTransient<IResultRequestHandler<CreateExportCommand, Result<Export>>, CreateExportCommandHandler>();
        services.AddTransient<IResultRequestHandler<ExportByIdQuery, Result<Export>>, ExportByIdQueryHandler>();
        services.AddTransient<IResultRequestHandler<StartExportCommand, Result<Export>>, StartExportCommandHandler>();
        services.AddTransient<IResultRequestHandler<FinalizeExportCommand, Result<Export>>, FinalizeExportCommandHandler>();

        services.AddSingleton<IValidator<CreateExportCommand>, CreateExportCommandValidator>();
        services.AddSingleton<IValidator<ExportByIdQuery>, ExportByIdQueryValidator>();
        services.AddSingleton<IValidator<StartExportCommand>, StartExportCommandValidator>();
        services.AddSingleton<IValidator<FinalizeExportCommand>, FinalizeExportCommandValidator>();


        services.AddSingleton<IOptions<ClientOptions>>(ctx => Options.Create<ClientOptions>(options));
        services.AddTransient<IClient, ExportClient>();

        return services;
    }
}
