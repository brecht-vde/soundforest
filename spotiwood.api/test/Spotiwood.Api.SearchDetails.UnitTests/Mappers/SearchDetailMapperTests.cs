using AutoMapper;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using Xunit;
using SearchDetailDto = Spotiwood.Integrations.Omdb.Domain.SearchDetail;
using SearchDetail = Spotiwood.Api.SearchDetails.Domain.SearchDetail;
using FluentAssertions;
using Spotiwood.Api.SearchDetails.Application.Mappers;


namespace Spotiwood.Api.SearchDetails.UnitTests.Mappers;
public sealed class SearchDetailMapperTests
{
    public static IEnumerable<object[]> ValidSearchDetail
    {
        get
        {
            yield return new object[]
            {
                new SearchDetailDto()
                {
                    EndYear = 2010,
                    StartYear = 1990,
                    Identifier = "tt1234567",
                    Poster = new Uri("http://images.local/file.jpg"),
                    Title = "Title",
                    Type = "movie",
                    Genres= new[] { "1", "2", "3" },
                    Plot = "Plot",
                    Seasons = 7,
                    Actors = new[] { "1", "2", "3" }
                },
                new SearchDetail()
                {
                    EndYear = 2010,
                    StartYear = 1990,
                    Identifier = "tt1234567",
                    Poster = new Uri("http://images.local/file.jpg"),
                    Title = "Title",
                    Type = "movie",
                    Genres= new[] { "1", "2", "3" },
                    Plot = "Plot",
                    Seasons = 7,
                    Actors = new[] { "1", "2", "3" }
                }
            };
        }
    }

    [Theory]
    [MemberData(nameof(ValidSearchDetail))]
    internal void SearchResultProfile_Map_Succeeds(SearchDetailDto input, SearchDetail output)
    {
        // Arrange
        var cfg = new MapperConfiguration(cfg => cfg.AddProfile<SearchDetailMapper>());
        var mapper = cfg.CreateMapper();

        // Act
        var result = mapper.Map<SearchDetail>(input);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeOfType<SearchDetail>();
            result.Should().BeEquivalentTo(output);
        }
    }
}
