using FluentValidation;
using Spotiwood.Api.Playlists.Application.Queries;

namespace Spotiwood.Api.Playlists.Application.Validators;
internal sealed class GetPlaylistsQueryValidator : AbstractValidator<GetPlaylistsQuery>
{
    public GetPlaylistsQueryValidator()
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
