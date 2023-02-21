using AutoFixture.Idioms;
using FluentAssertions;
using SoundForest.Framework.Authentication.UnitTests.Mocks;
using SoundForest.UnitTests.Common;
using SoundForest.Framework.Authentication.Application.Sentinels;
using System.Reflection;
using Xunit;

namespace SoundForest.Framework.Authentication.UnitTests.Sentinels;
public sealed class FunctionSentinelTests
{
    [Fact]
    public void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(FunctionSentinel).GetConstructors(BindingFlags.Public));
    }

    [Fact]
    public void FunctionSentinel_RegistrationMarker_RequiresElevation_Returns_True()
    {
        // Arrange
        var sut = new FunctionSentinel(typeof(MockFunction));

        // Act
        var result = sut.RequiresElevation(nameof(MockFunction));

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void FunctionSentinel_RegistrationAssembly_RequiresElevation_Returns_True()
    {
        // Arrange
        var sut = new FunctionSentinel(typeof(MockFunction).Assembly);

        // Act
        var result = sut.RequiresElevation(nameof(MockFunction));

        // Assert
        result.Should().BeTrue();
    }
}
