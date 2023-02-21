using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using SoundForest.Clients.Auth0.Authentication.Infrastructure;
using SoundForest.Clients.Auth0.Authentication.Infrastructure.Options;
using SoundForest.Clients.Auth0.Authentication.Infrastructure.Responses;
using SoundForest.UnitTests.Common;
using System.Text.Json;

namespace SoundForest.Clients.Auth0.UnitTests.Authentication.Clients;
public class Auth0ClientTests
{
    [Fact]
    internal void ConstructorGuards()
    {
        var assertion = new GuardClauseAssertion(AutoMoqDataAttribute.AutoMoqFixture);
        assertion.Verify(typeof(Auth0Client).GetConstructors());
    }

    [Theory]
    [AutoMoqData]
    internal async Task Auth0Client_M2mTokenAsync_Succeeds(
        ILogger<Auth0Client> logger,
        [Frozen] Mock<IOptions<Auth0Options>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        Auth0Options optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress,
        TokenResponse dto)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        handler.SetupAnyRequest()
            .ReturnsJsonResponse<TokenResponse>(dto);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        var sut = new Auth0Client(
                logger,
                options.Object,
                client,
                serializer.Object
            );

        // Act
        var result = await sut.M2mTokenAsync();

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.ExpiresIn.Should().Be(dto.expires_in);
            result!.AccessToken.Should().Be(dto.access_token);
        }
    }

    [Theory]
    [AutoMoqData]
    internal async Task Auth0Client_M2mTokenAsync_Fails(
        ILogger<Auth0Client> logger,
        [Frozen] Mock<IOptions<Auth0Options>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        Auth0Options optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        handler.SetupAnyRequest()
            .Throws<Exception>();

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        var sut = new Auth0Client(
                logger,
                options.Object,
                client,
                serializer.Object
            );

        // Act
        var result = await sut.M2mTokenAsync();

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [AutoMoqData]
    internal async Task Auth0Client_UserTokenAsync_Succeeds(
        ILogger<Auth0Client> logger,
        [Frozen] Mock<IOptions<Auth0Options>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        Auth0Options optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress,
        UserResponse dto,
        string username,
        string m2mToken)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        handler.SetupAnyRequest()
            .ReturnsJsonResponse<UserResponse>(dto);

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        var sut = new Auth0Client(
                logger,
                options.Object,
                client,
                serializer.Object
            );

        // Act
        var result = await sut.UserTokenAsync(m2mToken, username);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.AccessToken.Should().Be(dto.identities!.First().access_token);
            result!.Connection.Should().Be(dto.identities!.First().connection);
            result!.Id.Should().Be(dto.identities!.First().user_id);
            result!.IsSocial.Should().Be(dto.identities!.First().isSocial);
            result!.Provider.Should().Be(dto.identities!.First().provider);
            result!.RefreshToken.Should().Be(dto.identities!.First().refresh_token);
        }
    }

    [Theory]
    [AutoMoqData]
    internal async Task Auth0Client_UserTokenAsync_Fails(
        ILogger<Auth0Client> logger,
        [Frozen] Mock<IOptions<Auth0Options>> options,
        [Frozen] Mock<IOptions<JsonSerializerOptions>> serializer,
        Auth0Options optionsValue,
        Mock<HttpMessageHandler> handler,
        Uri baseAddress,
        string username,
        string m2mToken)
    {
        // Arrange
        options.Setup(opt => opt.Value).Returns(optionsValue);
        serializer.Setup(opt => opt.Value).Returns(DefaultJsonSerializerOptions.Options);

        handler.SetupAnyRequest()
            .Throws<Exception>();

        var client = handler.CreateClient();
        client.BaseAddress = baseAddress;

        var sut = new Auth0Client(
                logger,
                options.Object,
                client,
                serializer.Object
            );

        // Act
        var result = await sut.UserTokenAsync(m2mToken, username);

        // Assert
        result.Should().BeNull();
    }
}
