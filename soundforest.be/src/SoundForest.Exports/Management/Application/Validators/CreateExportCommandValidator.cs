using FluentValidation;
using SoundForest.Exports.Management.Application.Commands;
using SoundForest.Framework.Application.Validation;
using System.Text.RegularExpressions;

namespace SoundForest.Exports.Management.Application.Validators;
internal sealed class CreateExportCommandValidator : AbstractValidator<CreateExportCommand>
{
    public CreateExportCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
                .WithMessage("An identifier must be provided.")
            .IdentifierFormat()
                .WithMessage("An identifier must consist of 'tt' followed by 7 or 8 digits.");

        RuleFor(c => c.Username)
            .NotEmpty()
            .WithMessage("A username must be provided.");

        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("A name must be provided.");
    }
}
