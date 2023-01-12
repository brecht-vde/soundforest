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
public sealed class SearchResultProfileTests
{
    public static IEnumerable<object[]> ValidSearchResult
    {
        get
        {
            yield return new object[]
            {
                new SearchResultDto()
                {
                    ImdbID = "tt1234567",
                    Poster = "http://images.local/file.jpg",
                    Title = "Title",
                    Type = "movie",
                    Year = "1990–2010"
                },
                new SearchResult()
                {
                    EndYear = 2010,
                    StartYear = 1990,
                    Identifier = "tt1234567",
                    Poster = new Uri("http://images.local/file.jpg"),
                    Title = "Title",
                    Type = "movie"
                }
            };
        }
    }

    [Theory]
    [MemberData(nameof(ValidSearchResult))]
    internal void SearchResultProfile_Map_Succeeds(SearchResultDto input, SearchResult output)
    {
        // Arrange
        var cfg = new MapperConfiguration(cfg => cfg.AddProfile<SearchResultProfile>());
        var mapper = cfg.CreateMapper();

        // Act
        var result = mapper.Map<SearchResult>(input);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeOfType<SearchResult>();
            result.Should().BeEquivalentTo(output);
        }
    }
}
