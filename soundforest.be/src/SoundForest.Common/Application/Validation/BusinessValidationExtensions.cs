using FluentValidation;
using System.Text.RegularExpressions;

namespace SoundForest.Common.Application.Validation;
public static class BusinessValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> IdentifierFormat<T>(this IRuleBuilder<T, string?> builder)
    {
        return builder.Matches(new Regex(@"^tt\d{7,8}$"));
    }
}
