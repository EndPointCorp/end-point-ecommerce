// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.AdminPortal.Services;
using EndPointEcommerce.Tests.Fixtures;
using Moq;

namespace EndPointEcommerce.Tests.AdminPortal.Services;

public abstract class BaseEntitySearcherTests<TResultItem, TEntity> : TransactionalTests
{
    protected readonly Mock<IUrlBuilder> _mockUrlBuilder;
    protected readonly BaseEntitySearcher<TResultItem, TEntity> _subject;

    public BaseEntitySearcherTests(DatabaseFixture database) : base(database)
    {
        _mockUrlBuilder = new Mock<IUrlBuilder>();
        _mockUrlBuilder
            .Setup(m => m.Build(It.IsAny<string>(), It.IsAny<object>()))
            .Returns("test_edit_url");

        _subject = CreateSubject();
    }

    protected abstract BaseEntitySearcher<TResultItem, TEntity> CreateSubject();

    protected abstract void PopulateRecords();

    [Fact]
    public async Task Search_CanReturnUnfilteredData()
    {
        // Arrange
        PopulateRecords();

        var parameters = new SearchParameters
        {
            Draw = 1,
            Start = 0,
            Length = 100,
            Search = null,
            Order = null,
        };

        // Act
        var result = await _subject.Search(parameters, _mockUrlBuilder.Object);

        // Assert
        Assert.Equal(1, result.Draw);
        Assert.Equal(5, result.RecordsTotal);
        Assert.Equal(5, result.RecordsFiltered);
        Assert.Equal(5, result.Data.Count);
    }

    [Fact]
    public async Task Search_CanPaginate()
    {
        // Arrange
        PopulateRecords();

        var parameters = new SearchParameters
        {
            Draw = 1,
            Start = 0,
            Length = 3,
            Search = null,
            Order = null,
        };

        // Act
        var result = await _subject.Search(parameters, _mockUrlBuilder.Object);

        // Assert
        Assert.Equal(1, result.Draw);
        Assert.Equal(5, result.RecordsTotal);
        Assert.Equal(5, result.RecordsFiltered);
        Assert.Equal(3, result.Data.Count);
    }

    protected async Task RunSearchingTheory(string searchValue, Action<SearchResult<TResultItem>> asserter)
    {
        // Arrange
        PopulateRecords();

        var parameters = new SearchParameters
        {
            Draw = 1,
            Start = 0,
            Length = 100,
            Search = new Dictionary<string, string>
            {
                ["value"] = searchValue
            },
            Order = null,
        };

        // Act
        var result = await _subject.Search(parameters, _mockUrlBuilder.Object);

        // Assert
        Assert.Equal(1, result.Draw);
        Assert.Equal(5, result.RecordsTotal);
        Assert.Equal(1, result.RecordsFiltered);

        asserter.Invoke(result);
    }

    protected async Task RunSortingTheory(
        string orderByName, string orderByDirection,
        Action<SearchResult<TResultItem>> asserter
    ) {
        // Arrange
        PopulateRecords();

        var parameters = new SearchParameters
        {
            Draw = 1,
            Start = 0,
            Length = 100,
            Search = null,
            Order =
            [
                new Dictionary<string, string>()
                {
                    ["name"] = orderByName,
                    ["dir"] = orderByDirection
                }
            ],
        };

        // Act
        var result = await _subject.Search(parameters, _mockUrlBuilder.Object);

        // Assert
        Assert.Equal(1, result.Draw);
        Assert.Equal(5, result.RecordsTotal);
        Assert.Equal(5, result.RecordsFiltered);
        Assert.Equal(5, result.Data.Count);

        asserter.Invoke(result);
    }
}
