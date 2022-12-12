using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Spotiwood.Api.Search.Application.Mappers;
using Spotiwood.Framework.Application.Pagination;
using Spotiwood.Integrations.Omdb.Domain;
using System;
using System.Collections.Generic;
using Xunit;
using SearchResult = Spotiwood.Api.Search.Domain.SearchResult;
using SearchResultDto = Spotiwood.Integrations.Omdb.Domain.SearchResult;

namespace Spotiwood.Api.Search.UnitTests.Mappers;
public sealed class SearchResultsMapperTest
{
    public static IEnumerable<object[]> ValidSearchResultCollection
    {
        get
        {
            yield return new object[]
            {
                new SearchResultCollection()
                {

                    Page = 1,
                    PageSize = 10,
                    Total = 1,
                    Results = new SearchResultDto[]
                    {
                        new SearchResultDto()
                        {
                            EndYear = 2010,
                            StartYear = 1990,
                            Identifier = "tt1234567",
                            Poster = new Uri("http://images.local/file.jpg"),
                            Title = "Title",
                            Type = "movie"
                        }
                    }
                },
                new PagedCollection<SearchResult>()
                {
                    Page = 1,
                    Size = 10,
                    Total = 1,
                    Items = new SearchResult[]
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
    internal void SearchResultCollectionProfile_Map_Succeeds(SearchResultCollection input, PagedCollection<SearchResult> output)
    {
        // Arrange
        var cfg = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SearchResultMapper>();
            cfg.AddProfile<SearchResultsMapper>();
        });

        var mapper = cfg.CreateMapper();

        // Act
        var result = mapper.Map<PagedCollection<SearchResult>>(input);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeOfType<PagedCollection<SearchResult>>();
            result.Should().BeEquivalentTo(output);
        }
    }
}
