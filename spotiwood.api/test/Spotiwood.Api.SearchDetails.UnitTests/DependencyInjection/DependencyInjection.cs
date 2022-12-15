using AutoMapper;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Spotiwood.Api.SearchDetails.Application.Queries;
using Spotiwood.Api.SearchDetails.Domain;
using Spotiwood.Framework.Application.Requests;
using Spotiwood.UnitTests.Common;
using System;
using Xunit;

namespace Spotiwood.Api.SearchDetails.UnitTests.DependencyInjection;
public sealed class DependencyInjection
{
    [Theory]
    [AutoMoqData]
    public void ServiceCollection_HasAllDependencies_Succeeds(Uri uri, string key)
    {
        // Arrange
        var sut = new ServiceCollection();

        // Act
        sut.AddSearchDetails(uri, key);
        var provider = sut.BuildServiceProvider();

        // Assert
        using (new AssertionScope())
        {
            provider.GetRequiredService<IResultRequestHandler<SearchByIdQuery, Result<SearchDetail>>>()
                .Should().NotBeNull();

            provider.GetRequiredService<IValidator<SearchByIdQuery>>()
                .Should().NotBeNull();

            provider.GetRequiredService<IMapper>()
                .Should().NotBeNull();
        }
    }
}
