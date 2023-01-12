using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Spotiwood.Api.Search.Application.Mappers;
using System;
using System.Collections.Generic;
using Xunit;
using SearchResult = Spotiwood.Api.Search.Domain.SearchResult;
using SearchResultDto = Spotiwood.Integrations.Omdb.Domain.SearchResult;

namespace Spotiwood.Api.Search.UnitTests.Mappers;
public sealed class SearchResultMapperTests
{
    public static IEnumerable<object[]> ValidSearchResult
    {
        get
        {
            yield return new object[]
            {
                new SearchResultDto()
                {
                    EndYear = 2010,
                    StartYear = 1990,
                    Identifier = "tt1234567",
                    Poster = new Uri("http://images.local/file.jpg"),
                    Title = "Title",
                    Type = "movie"
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
        var cfg = new MapperConfiguration(cfg => cfg.AddProfile<SearchResultMapper>());
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
