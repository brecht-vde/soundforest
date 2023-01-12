using FluentValidation;
using SoundForest.Playlists.Application.Queries;
using System.Text.RegularExpressions;

namespace SoundForest.Playlists.Application.Validators;
internal sealed class PlaylistByIdQueryValidator : AbstractValidator<PlaylistByIdQuery>
{
    public PlaylistByIdQueryValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty()
                .WithMessage("An identifier must be provided.")
            .Matches(new Regex(@"^tt\d{7,8}$"))
                .WithMessage("An identifier must consist of 'tt' followed by 7 or 8 digits.");
    }
}
