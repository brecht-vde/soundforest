using FluentValidation;
using SoundForest.Framework.Application.Validation;
using SoundForest.Titles.Management.Application.Queries;
using System.Text.RegularExpressions;

namespace SoundForest.Titles.Management.Application.Validators;
internal sealed class SearchByIdQueryValidator : AbstractValidator<SearchByIdQuery>
{
    public SearchByIdQueryValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty()
                .WithMessage("An identifier must be provided.")
            .IdentifierFormat()
                .WithMessage("An identifier must consist of 'tt' followed by 7 or 8 digits.");
    }
}
