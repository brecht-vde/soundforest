using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Spotiwood.Framework.Application.Pagination;
using Spotiwood.Api.Search.Application.Queries;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.Api.Search.Domain;
using Spotiwood.UnitTests.Common;
using System;
using Xunit;

namespace Spotiwood.Api.Search.UnitTests.DependencyInjection;
public sealed class DependencyInjectionTests
{
    [Theory]
    [AutoMoqData]
    public void ServiceCollection_HasAllDependencies_Succeeds(Uri uri, string key)
    {
        // Arrange
        var sut = new ServiceCollection();

        // Act
        sut.AddSearch(uri, key);
        var provider = sut.BuildServiceProvider();

        // Assert
        using (new AssertionScope())
        {
            // TODO: how to check pipeline behavior?

            provider.GetRequiredService<IResultRequestHandler<FreeTextSearchQuery, Result<PagedCollection<SearchResult>>>>()
                .Should().NotBeNull();

            provider.GetRequiredService<IValidator<FreeTextSearchQuery>>()
                .Should().NotBeNull();

            provider.GetRequiredService<IMapper>()
                .Should().NotBeNull();
        }
    }
}
