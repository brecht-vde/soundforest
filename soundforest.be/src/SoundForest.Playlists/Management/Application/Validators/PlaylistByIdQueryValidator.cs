using FluentValidation;
using SoundForest.Framework.Application.Validation;
using SoundForest.Playlists.Management.Application.Queries;
using System.Text.RegularExpressions;

namespace SoundForest.Playlists.Management.Application.Validators;
internal sealed class PlaylistByIdQueryValidator : AbstractValidator<PlaylistByIdQuery>
{
    public PlaylistByIdQueryValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty()
                .WithMessage("An identifier must be provided.")
            .IdentifierFormat()
                .WithMessage("An identifier must consist of 'tt' followed by 7 or 8 digits.");
    }
}
