using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Moq;
using Spotiwood.Framework.Authentication.Application.Abstractions;
using Spotiwood.Framework.Authentication.Infrastructure.Options;
using Spotiwood.UnitTests.Common;
using System;
using System.Reflection;
using Xunit;

namespace Spotiwood.Framework.Authentication.UnitTests.DependencyInjection;
public sealed class ServiceCollectionTests
{
    [Theory]
    [AutoMoqData]
    public void AddFunctionAuthentication_WithAssemblies_Succeeds(Uri issuer, string audience)
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddFunctionAuthentication(audience, issuer, typeof(IAuthenticator).Assembly);

        // Act
        var sut = services.BuildServiceProvider();

        // Assert
        using (new AssertionScope())
        {
            sut.GetRequiredService<IOptions<AuthenticationOptions>>().Should().NotBeNull();
            sut.GetRequiredService<IAuthenticator>().Should().NotBeNull();
            sut.GetRequiredService<IConfigurationManager<OpenIdConnectConfiguration>>().Should().NotBeNull();
            sut.GetRequiredService<IFunctionSentinel>().Should().NotBeNull();
        }
    }

    [Theory]
    [AutoMoqData]
    public void ServiceCollection_EmptyIssuer_Throws(string audience, Assembly assembly)
    {
        // Arrange
        var sut = new ServiceCollection();

        // Act
        var action = () => sut.AddFunctionAuthentication(audience, It.IsAny<Uri>(), assembly);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoMoqData]
    public void ServiceCollection_EmptyAudience_Throws(Uri issuer, Assembly assembly)
    {
        // Arrange
        var sut = new ServiceCollection();

        // Act
        var action = () => sut.AddFunctionAuthentication(It.IsAny<string>(), issuer, assembly);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoMoqData]
    public void ServiceCollection_Assemblies_Throws(string audience, Uri issuer)
    {
        // Arrange
        var sut = new ServiceCollection();

        // Act
        var action = () => sut.AddFunctionAuthentication(audience, issuer);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }
}
