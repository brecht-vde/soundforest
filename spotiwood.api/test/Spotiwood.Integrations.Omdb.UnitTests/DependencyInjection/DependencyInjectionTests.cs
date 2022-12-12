using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Spotiwood.Integrations.Omdb.Application.Abstractions;
using Spotiwood.Integrations.Omdb.Application.Options;
using Spotiwood.UnitTests.Common;
using System;
using Xunit;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

namespace Spotiwood.Integrations.Omdb.UnitTests.DependencyInjection;
public sealed class DependencyInjectionTests
{
    [Theory]
    [AutoMoqData]
    public void ServiceCollection_HasAllDependencies_Succeeds(Uri uri, string key)
    {
        // Arrange
        var sut = new ServiceCollection();

        // Act
        sut.AddOmdb(uri, key);
        var provider = sut.BuildServiceProvider();

        // Assert
        using (new AssertionScope())
        {
            provider.GetRequiredService<IOptions<ClientOptions>>()
                .Should().NotBeNull();

            provider.GetRequiredService<IClient>()
                .Should().NotBeNull();

            provider.GetRequiredService<IDbClient>()
                .Should().NotBeNull();

            provider.GetRequiredService<IMapper>()
                .Should().NotBeNull();

            provider.GetRequiredService<ILoggerFactory>()
                .Should().NotBeNull();
        }
    }

    [Fact]
    public void ServiceCollection_EmptyUri_Throws()
    {
        // Arrange
        var sut = new ServiceCollection();

        // Act
        var action = () => sut.AddOmdb(It.IsAny<Uri>(), It.IsNotNull<string>());

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [AutoMoqData]
    public void ServiceCollection_EmptyKey_Throws(Uri uri)
    {
        // Arrange
        var sut = new ServiceCollection();

        // Act
        var action = () => sut.AddOmdb(uri, It.IsAny<string>());

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ServiceCollection_EmptyClientOptions_Throws()
    {
        // Arrange
        var sut = new ServiceCollection();

        // Act
        var action = () => sut.AddOmdb(It.IsAny<ClientOptions>());

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }
}
