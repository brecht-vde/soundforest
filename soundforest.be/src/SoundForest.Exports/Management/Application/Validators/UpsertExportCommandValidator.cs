using FluentValidation;
using SoundForest.Exports.Management.Application.Commands;
using System.Text.RegularExpressions;

namespace SoundForest.Exports.Management.Application.Validators;
internal sealed class UpsertExportCommandValidator : AbstractValidator<UpsertExportCommand>
{
    public UpsertExportCommandValidator()
    {
        RuleFor(c => c.Properties)
            .NotEmpty();

        RuleFor(c => c.Id)
            .NotEmpty()
                .WithMessage("An identifier must be provided.")
            .Matches(new Regex(@"^tt\d{7,8}$"))
                .WithMessage("An identifier must consist of 'tt' followed by 7 or 8 digits.");
    }
}