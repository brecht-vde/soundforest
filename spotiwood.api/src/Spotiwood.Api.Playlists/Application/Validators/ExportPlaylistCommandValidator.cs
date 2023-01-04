using FluentValidation;
using Spotiwood.Api.Playlists.Application.Commands;
using System.Text.RegularExpressions;

namespace Spotiwood.Api.Playlists.Application.Validators;
internal sealed class ExportPlaylistCommandValidator : AbstractValidator<ExportPlaylistCommand>
{
    public ExportPlaylistCommandValidator()
    {
        RuleFor(c => c.Identifier)
            .NotEmpty()
                .WithMessage("An identifier must be provided.")
            .Matches(new Regex(@"^tt\d{7,8}$"))
                .WithMessage("An identifier must consist of 'tt' followed by 7 or 8 digits.");

        RuleFor(c => c.Title)
            .NotEmpty()
                .WithMessage("A title must be provided.");
    }
}
