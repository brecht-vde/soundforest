using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using Spotiwood.Api.Search.Application.Behaviors;
using Spotiwood.Framework.Application.Pagination;
using Spotiwood.Api.Search.Application.Queries;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.Api.Search.Domain;
using Spotiwood.UnitTests.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Spotiwood.Api.Search.UnitTests.Behaviors;
public sealed class ResultRequestValidationBehaviorTests
{

    [Fact]
    public void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(ResultRequestValidationBehavior<FreeTextSearchQuery, Result<PagedCollection<SearchResult>>>).GetConstructors());
    }

    [Theory]
    [AutoMoqData]
    internal async Task ResultRequestValidationBehavior_Fails(
        [Frozen] Mock<IValidator<FreeTextSearchQuery>> validator,
        [Frozen] Mock<IEnumerable<IValidator<FreeTextSearchQuery>>> validators,
        IEnumerable<ValidationFailure> errors,
        FreeTextSearchQuery query,
        ResultRequestValidationBehavior<FreeTextSearchQuery, Result<PagedCollection<SearchResult>>> sut)
    {
        // Arrange
        validator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(errors));

        validators.SetReturnsDefault(new[] { validator });

        // Act
        var result = await sut.Handle(query, It.IsAny<RequestHandlerDelegate<Result<PagedCollection<SearchResult>>>>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(400);
    }

    [Theory]
    [AutoMoqData]
    internal async Task ResultRequestValidationBehavior_Succeeds(
        [Frozen] Mock<IValidator<FreeTextSearchQuery>> validator,
        [Frozen] Mock<IEnumerable<IValidator<FreeTextSearchQuery>>> validators,
        FreeTextSearchQuery query,
        Mock<RequestHandlerDelegate<Result<PagedCollection<SearchResult>>>> next,
        PagedCollection<SearchResult> results,
        ResultRequestValidationBehavior<FreeTextSearchQuery, Result<PagedCollection<SearchResult>>> sut)
    {
        // Arrange
        validator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        validators.SetReturnsDefault(new[] { validator });

        next.Setup(n => n())
            .ReturnsAsync(Result<PagedCollection<SearchResult>>.SuccessResult(results));

        // Act
        var result = await sut.Handle(query, next.Object, It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }
}
