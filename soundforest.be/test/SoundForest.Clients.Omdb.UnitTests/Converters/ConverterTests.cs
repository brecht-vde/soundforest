using FluentAssertions;
using SoundForest.Clients.Omdb.Search.Infrastructure.Mappings;
using System.Linq;
using Xunit;

namespace SoundForest.Clients.Omdb.UnitTests.Converters;
public sealed class ConverterTests
{
    [Theory]
    [InlineData("1990")]
    public void ToStartYear_Succeeds(string value)
    {
        var result = value.ToStartYear();
        result.Should()
            .NotBeNull()
            .And
            .Be(1990);
    }

    [Theory]
    [InlineData("1990–1991")]
    public void ToEndYear_Succeeds(string value)
    {
        var result = value.ToEndYear();
        result.Should()
            .NotBeNull()
            .And
            .Be(1991);
    }

    [Theory]
    [InlineData("http://unittests.local")]
    public void ToUri_Succeeds(string value)
    {
        var result = value.ToUri();
        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("123")]
    public void ToInt_Succeeds(string value)
    {
        var result = value.ToInt();
        result.Should()
            .NotBeNull()
            .And
            .Be(123);
    }

    [Theory]
    [InlineData("item 1, item 2, item 3")]
    public void ToList_Succeeds(string value)
    {
        var result = value.ToList();
        result.Should()
            .NotBeNull()
            .And
            .HaveCount(3);
    }
}