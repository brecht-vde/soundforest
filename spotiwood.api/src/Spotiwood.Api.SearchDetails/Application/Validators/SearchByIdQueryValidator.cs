using FluentValidation;
using Spotiwood.Api.SearchDetails.Application.Queries;
using System.Text.RegularExpressions;

namespace Spotiwood.Api.SearchDetails.Application.Validators;
internal sealed class SearchByIdQueryValidator : AbstractValidator<SearchByIdQuery>
{
    public SearchByIdQueryValidator()
    {
        RuleFor(q => q.Identifier)
            .NotEmpty()
                .WithMessage("An identifier must be provided.")
            .Matches(new Regex(@"^tt\d{7,8}$"))
                .WithMessage("An identifier must consist of 'tt' followed by 7 or 8 digits.");
    }
}