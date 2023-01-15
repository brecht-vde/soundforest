using SoundForest.Exports.Management.Application.Commands;
using SoundForest.Exports.Management.Domain;

namespace SoundForest.Exports.Management.Application.Mappers;
internal static class Mapper
{
    public static Export ToExport(this CreateExportCommand command)
        => new Export(
            Id: command?.Id,
            Name: command?.Name,
            Username: command?.Username,
            Status: Status.Pending,
            ExternalId: null
            );
}
