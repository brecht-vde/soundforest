using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Spotiwood.Integrations.Omdb.Application.Dtos;
using Spotiwood.Integrations.Omdb.Application.Mappers;
using Spotiwood.Integrations.Omdb.Domain;
using System;
using System.Collections.Generic;
using Xunit;

namespace Spotiwood.Integrations.Omdb.UnitTests.Mappers;
public sealed class SearchDetailProfileTests
{
    public static IEnumerable<object[]> ValidSearchDetail
    {
        get
        {
            yield return new object[]
            {
                new SearchDetailDto()
                {
                    // Mapped props
                    ImdbID = "tt1234567",
                    Poster = "http://images.local/file.jpg",
                    Title = "Title",
                    Type = "movie",
                    Year = "1990–2010",
                    TotalSeasons = "7",
                    Genre = "1, 2, 3",
                    Plot = "Some plot",
                    Actors = "1, 2, 3",

                    // Unmapped props
                    Awards = "N/A",
                    BoxOffice = "N/A",
                    Country = "N/A",
                    Director = "N/A",
                    DVD = "N/A",
                    ImdbRating = "N/A",
                    ImdbVotes = "N/A",
                    Language = "N/A",
                    Metascore = "N/A",
                    Production = "N/A",
                    Rated = "N/A",
                    Ratings = new List<SearchDetailRatingDto>()
                    {
                        new SearchDetailRatingDto()
                        {
                            Source = "N/A",
                            Value = "N/A"
                        }
                    },
                    Released = "N/A",
                    Response = "N/A",
                    Runtime = "N/A",
                    Website = "N/A",
                    Writer = "N/A"
                },
                new SearchDetail()
                {
                    EndYear = 2010,
                    StartYear = 1990,
                    Identifier = "tt1234567",
                    Poster = new Uri("http://images.local/file.jpg"),
                    Title = "Title",
                    Type = "movie",
                    Seasons = 7,
                    Genres = new[] { "1", "2", "3" },
                    Plot = "Some plot",
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
        var cfg = new MapperConfiguration(cfg => cfg.AddProfile<SearchDetailProfile>());
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
