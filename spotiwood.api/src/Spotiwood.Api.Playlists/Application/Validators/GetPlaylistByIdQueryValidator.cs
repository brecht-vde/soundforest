using FluentValidation;
using Spotiwood.Api.Playlists.Application.Queries;
using System.Text.RegularExpressions;

namespace Spotiwood.Api.Playlists.Application.Validators;
internal sealed class GetPlaylistByIdQueryValidator : AbstractValidator<GetPlaylistByIdQuery>
{
    public GetPlaylistByIdQueryValidator()
    {
        RuleFor(q => q.Identifier)
            .NotEmpty()
                .WithMessage("An identifier must be provided.")
            .Matches(new Regex(@"^tt\d{7,8}$"))
                .WithMessage("An identifier must consist of 'tt' followed by 7 or 8 digits.");
    }
}
