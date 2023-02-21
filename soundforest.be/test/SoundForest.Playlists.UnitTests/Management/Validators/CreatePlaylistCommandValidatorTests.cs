using FluentAssertions;
using Moq;
using SoundForest.Playlists.Management.Application.Commands;
using SoundForest.Playlists.Management.Application.Validators;
using SoundForest.UnitTests.Common;
using Xunit;

namespace SoundForest.Playlists.UnitTests.Management.Validators;
public class CreatePlaylistCommandValidatorTests
{
    [Theory]
    [AutoMoqData]
    internal void Validator_Succeeds(CreatePlaylistCommand command)
    {
        // Arrange
        var sut = new CreatePlaylistCommandValidator();

        // Act
        var result = sut.Validate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }
}
