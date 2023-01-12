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
public sealed class SearchResultCollectionProfileTests
{
    public static IEnumerable<object[]> ValidSearchResultCollection
    {
        get
        {
            yield return new object[]
            {
                new SearchResultDtoCollection()
                {
                    Page = 1,
                    PageSize = 10,
                    Response = "True",
                    TotalResults = "1",
                    Search = new SearchResultDto[]
                    {
                        new SearchResultDto()
                        {
                            ImdbID = "tt1234567",
                            Poster = "http://images.local/file.jpg",
                            Title = "Title",
                            Type = "movie",
                            Year = "1990–2010"
                        }
                    }
                },
                new SearchResultCollection()
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 1,
                    Results = new SearchResult[]
                    {
                        new SearchResult()
                        {
                            EndYear = 2010,
                            StartYear = 1990,
                            Identifier = "tt1234567",
                            Poster = new Uri("http://images.local/file.jpg"),
                            Title = "Title",
                            Type = "movie"
                        }
                    }
                }
            };
        }
    }

    [Theory]
    [MemberData(nameof(ValidSearchResultCollection))]
    internal void SearchResultCollectionProfile_Map_Succeeds(SearchResultDtoCollection input, SearchResultCollection output)
    {
        // Arrange
        var cfg = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SearchResultCollectionProfile>();
            cfg.AddProfile<SearchResultProfile>();
        });

        var mapper = cfg.CreateMapper();

        // Act
        var result = mapper.Map<SearchResultCollection>(input);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeOfType<SearchResultCollection>();
            result.Should().BeEquivalentTo(output);
        }
    }
}
