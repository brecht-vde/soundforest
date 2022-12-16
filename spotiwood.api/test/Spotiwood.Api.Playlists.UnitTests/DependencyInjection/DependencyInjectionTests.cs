using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Spotiwood.Api.Playlists.Application.Queries;
using Spotiwood.Api.Playlists.Domain;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.UnitTests.Common;
using Xunit;

namespace Spotiwood.Api.Playlists.UnitTests.DependencyInjection;
public sealed class DependencyInjectionTests
{
    private static string _connectionString = "AccountName=unittests;AccountKey=dW5pdHRlc3Rz;DefaultEndpointsProtocol=http;TableEndpoint=http://unittests/unittests;";

    [Fact]
    public void ServiceCollection_HasAllDependencies_Succeeds()
    {
        // Arrange
        var sut = new ServiceCollection();

        // Act
        sut.AddPlaylists(_connectionString);
        var provider = sut.BuildServiceProvider();

        // Assert
        using (new AssertionScope())
        {
            provider.GetRequiredService<IResultRequestHandler<GetPlaylistByIdQuery, Result<Playlist>>>()
                .Should().NotBeNull();

            provider.GetRequiredService<IValidator<GetPlaylistByIdQuery>>()
                .Should().NotBeNull();

            provider.GetRequiredService<IMapper>()
                .Should().NotBeNull();
        }
    }
}
