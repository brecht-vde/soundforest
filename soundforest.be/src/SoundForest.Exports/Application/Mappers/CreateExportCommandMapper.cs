using SoundForest.Exports.Application.Commands;
using SoundForest.Exports.Domain;

namespace SoundForest.Exports.Application.Mappers;
internal static class CreateExportCommandMapper
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
