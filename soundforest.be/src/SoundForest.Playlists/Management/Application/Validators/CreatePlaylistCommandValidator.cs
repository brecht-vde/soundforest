using FluentValidation;
using SoundForest.Playlists.Management.Application.Commands;

namespace SoundForest.Playlists.Management.Application.Validators;
internal sealed class CreatePlaylistCommandValidator : AbstractValidator<CreatePlaylistCommand>
{
    public CreatePlaylistCommandValidator()
    {
        RuleFor(c => c.Playlist)
            .NotEmpty()
            .WithMessage("A valid playlist must be provided.");
    }
}
