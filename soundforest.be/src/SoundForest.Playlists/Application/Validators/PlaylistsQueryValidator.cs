using FluentValidation;
using SoundForest.Playlists.Application.Queries;

namespace SoundForest.Playlists.Application.Validators;
internal sealed class PlaylistsQueryValidator : AbstractValidator<PlaylistsQuery>
{
    public PlaylistsQueryValidator()
    {
        RuleFor(q => q.Size)
            .GreaterThan(0)
            .When(q => q.Size is not null)
            .WithMessage("Size must be greater than 0.");

        RuleFor(q => q.Page)
            .GreaterThan(0)
            .When(q => q.Page is not null)
            .WithMessage("Page must be greater than 0.");
    }
}
