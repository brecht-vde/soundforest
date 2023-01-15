using FluentValidation;
using SoundForest.Titles.Management.Application.Queries;

namespace SoundForest.Titles.Management.Application.Validators;
internal sealed class FreeTextSearchQueryValidator : AbstractValidator<FreeTextSearchQuery>
{
    public FreeTextSearchQueryValidator()
    {
        RuleFor(p => p.Query)
            .NotEmpty()
            .WithMessage("A query must be provided.");

        RuleFor(p => p.Page)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("A page number greater than 0 must be provided.");
    }
}
