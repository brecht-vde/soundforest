using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Spotiwood.Api.Playlists.Application.Queries;
using Spotiwood.Api.Playlists.Domain;
using Spotiwood.Api.Playlists.Infrastructure.Extensions;
using Spotiwood.Framework.Application.Requests;
using Xunit;

namespace Spotiwood.Api.Playlists.UnitTests.DependencyInjection;
public sealed class DependencyInjectionTests
{
    private static string _connectionString = "AccountEndpoint=https://unittests.local;AccountKey=dW5pdHRlc3Rz";

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

            provider.GetRequiredService<ICosmosQueryBuilder>()
                .Should().NotBeNull();
        }
    }
}
