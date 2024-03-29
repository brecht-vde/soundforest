﻿using SoundForest.Exports.Management.Domain;
using SoundForest.Schema;
using SoundForest.Schema.Exports;

namespace SoundForest.Exports.Management.Infrastructure.Mappers;
internal static class ExportMapper
{
    public static Export ToExport(this ExportEntity entity)
        => new Export(
                Id: entity?.id,
                Name: entity?.name,
                Username: entity?.username,
                Status: entity?.status is not null ? entity.status.ToStatus() : Status.NA,
                ExternalId: entity?.externalId,
                Log: entity?.log
            );

    public static ExportEntity ToExportEntity(this Export export)
        => new ExportEntity(
                id: export?.Id,
                name: export?.Name,
                username: export?.Username,
                status: export?.Status.FromStatus(),
                externalId: export?.ExternalId,
                log: export?.Log
            );

    public static string FromStatus(this Status status)
        => status switch
        {
            Status.Failed => Constants.Statuses.Failed,
            Status.Pending => Constants.Statuses.Pending,
            Status.Running => Constants.Statuses.Running,
            Status.Completed => Constants.Statuses.Completed,
            Status.Finalizing => Constants.Statuses.Finalizing,
            _ => Constants.Statuses.NA
        };

    public static Status ToStatus(this string? status)
        => status?.ToLower() switch
        {
            Constants.Statuses.Failed => Status.Failed,
            Constants.Statuses.Pending => Status.Pending,
            Constants.Statuses.Running => Status.Running,
            Constants.Statuses.Completed => Status.Completed,
            Constants.Statuses.Finalizing => Status.Finalizing,
            _ => Status.NA
        };
}
