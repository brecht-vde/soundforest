using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using Spotiwood.Framework.Application.Behaviors;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.UnitTests.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Spotiwood.Framework.UnitTests.Behaviors;
public sealed class ResultRequestValidationBehaviorTests
{
    public sealed record MockRequest(string MockProperty) : IResultRequest<Result<string>>;

    [Fact]
    public void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(ResultRequestValidationBehavior<MockRequest, Result<string>>).GetConstructors());
    }

    [Theory]
    [AutoMoqData]
    internal async Task ResultRequestValidationBehavior_Fails(
        [Frozen] Mock<IValidator<MockRequest>> validator,
        [Frozen] Mock<IEnumerable<IValidator<MockRequest>>> validators,
        IEnumerable<ValidationFailure> errors,
        MockRequest request,
        ResultRequestValidationBehavior<MockRequest, Result<string>> sut)
    {
        // Arrange
        validator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(errors));

        validators.SetReturnsDefault(new[] { validator });

        // Act
        var result = await sut.Handle(request, It.IsAny<RequestHandlerDelegate<Result<string>>>(), It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeFalse();
        result.StatusCode.Should().Be(400);
    }

    [Theory]
    [AutoMoqData]
    internal async Task ResultRequestValidationBehavior_Succeeds(
        [Frozen] Mock<IValidator<MockRequest>> validator,
        [Frozen] Mock<IEnumerable<IValidator<MockRequest>>> validators,
        MockRequest request,
        Mock<RequestHandlerDelegate<Result<string>>> next,
        string res,
        ResultRequestValidationBehavior<MockRequest, Result<string>> sut)
    {
        // Arrange
        validator.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        validators.SetReturnsDefault(new[] { validator });

        next.Setup(n => n())
            .ReturnsAsync(Result<string>.SuccessResult(res));

        // Act
        var result = await sut.Handle(request, next.Object, It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
        result.IsSuccessful.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }
}
