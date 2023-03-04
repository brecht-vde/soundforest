using FluentValidation;
using SoundForest.Exports.Processing.Application.Commands;
using SoundForest.Framework.Application.Validation;

namespace SoundForest.Exports.Processing.Application.Validators;
internal sealed class ProcessExportCommandValidator : AbstractValidator<ProcessExportCommand>
{
    public ProcessExportCommandValidator()
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
