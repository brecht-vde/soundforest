using FluentAssertions;
using Spotiwood.Api.Search.Application.Queries;
using Spotiwood.Api.Search.Application.Validators;
using System.Collections.Generic;
using Xunit;

namespace Spotiwood.Api.Search.UnitTests.Validators;
public sealed class FreeTextSearchQueryValidatorTests
{
    public static IEnumerable<object[]> ValidQueries
    {
        get
        {
            yield return new object[] { new FreeTextSearchQuery("N/A") };
            yield return new object[] { new FreeTextSearchQuery("N/A", 1) };
        }
    }

    [Theory]
    [MemberData(nameof(ValidQueries))]
    public void Validator_Succeeds(FreeTextSearchQuery query)
    {
        // Arrange
        var sut = new FreeTextSearchQueryValidator();

        // Act
        var result = sut.Validate(query);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> InvalidQueries
    {
        get
        {
            yield return new object[] { new FreeTextSearchQuery("") };
            yield return new object[] { new FreeTextSearchQuery(" ") };
            yield return new object[] { new FreeTextSearchQuery("   ") };
            yield return new object[] { new FreeTextSearchQuery("", 0) };
            yield return new object[] { new FreeTextSearchQuery("", -1) };
            yield return new object[] { new FreeTextSearchQuery("N/A", 0) };
            yield return new object[] { new FreeTextSearchQuery("N/A", -1) };
        }
    }

    [Theory]
    [MemberData(nameof(InvalidQueries))]
    public void Validator_Fails(FreeTextSearchQuery query)
    {
        // Arrange
        var sut = new FreeTextSearchQueryValidator();

        // Act
        var result = sut.Validate(query);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
    }
}
