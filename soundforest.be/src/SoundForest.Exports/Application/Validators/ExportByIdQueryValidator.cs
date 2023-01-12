using FluentValidation;
using SoundForest.Exports.Application.Queries;
using System.Text.RegularExpressions;

namespace SoundForest.Exports.Application.Validators;
internal sealed class ExportByIdQueryValidator : AbstractValidator<ExportByIdQuery>
{
    public ExportByIdQueryValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
                .WithMessage("An identifier must be provided.")
            .Matches(new Regex(@"^tt\d{7,8}$"))
                .WithMessage("An identifier must consist of 'tt' followed by 7 or 8 digits.");
    }
}
