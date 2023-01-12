using FluentValidation;
using SoundForest.Playlists.Application.Commands;

namespace SoundForest.Playlists.Application.Validators;
internal sealed class CreatePlaylistCommandValidator : AbstractValidator<CreatePlaylistCommand>
{
    public CreatePlaylistCommandValidator()
    {
        RuleFor(c => c.Playlist)
            .NotEmpty()
            .WithMessage("A valid playlist must be provided.");
    }
}
