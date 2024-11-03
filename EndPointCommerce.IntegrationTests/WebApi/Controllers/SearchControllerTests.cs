using System.Net;
using System.Net.Http.Json;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.IntegrationTests.Fixtures;
using EndPointCommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EndPointCommerce.IntegrationTests.WebApi.Controllers;

public class SearchControllerTests : IntegrationTestFixture
{
    public SearchControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture database) :
        base(factory, database) { }

    private Product CreateNewProduct(string name, string sku, string description)
    {
        var newProduct = new Product() {
            Name = name,
            Sku = sku,
            Description = description,
            BasePrice = 10.00M
        };

        _dbContext.Products.Add(newProduct);
        _dbContext.SaveChanges();

        return newProduct;
    }

    [Fact]
    public async Task GetSearchProducts_ReturnsMatchingProducts_WhenSearchingByName()
    {
        await WithTransaction(async () => {
            // Arrange
            CreateNewProduct("test product 1", "TP1", "This is the first product description");
            CreateNewProduct("test product 2", "TP2", "This is the second product description");
            CreateNewProduct("another product", "AP1", "This product is unrelated");

            var client = CreateHttpClient();

            // Act
            var response = await client.GetAsync("/api/Search/Products/test");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var products = await response.Content.ReadFromJsonAsync<IEnumerable<EndPointCommerce.WebApi.ResourceModels.Product>>();

            Assert.NotNull(products);
            Assert.Equal(2, products.Count());
            Assert.Contains(products, p => p.Name == "test product 1");
            Assert.Contains(products, p => p.Name == "test product 2");
            Assert.DoesNotContain(products, p => p.Name == "another product");
        });
    }

    [Fact]
    public async Task GetSearchProducts_ReturnsMatchingProducts_WhenSearchingBySku()
    {
        await WithTransaction(async () => {
            // Arrange
            CreateNewProduct("test product 1", "TP1", "This is the first product description");
            CreateNewProduct("test product 2", "TP2", "This is the second product description");
            CreateNewProduct("another product", "AP1", "This product is unrelated");

            var client = CreateHttpClient();

            // Act
            var response = await client.GetAsync("/api/Search/Products/TP");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var products = await response.Content.ReadFromJsonAsync<IEnumerable<EndPointCommerce.WebApi.ResourceModels.Product>>();

            Assert.NotNull(products);
            Assert.Equal(2, products.Count());
            Assert.Contains(products, p => p.Sku == "TP1");
            Assert.Contains(products, p => p.Sku == "TP2");
            Assert.DoesNotContain(products, p => p.Sku == "AP1");
        });
    }

    [Fact]
    public async Task GetSearchProducts_ReturnsMatchingProducts_WhenSearchingByDescription()
    {
        await WithTransaction(async () => {
            // Arrange
            CreateNewProduct("test product 1", "TP1", "This is the first product description");
            CreateNewProduct("test product 2", "TP2", "This is the second product description");
            CreateNewProduct("another product", "AP1", "This product is unrelated");

            var client = CreateHttpClient();

            // Act
            var response = await client.GetAsync("/api/Search/Products/description");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var products = await response.Content.ReadFromJsonAsync<IEnumerable<EndPointCommerce.WebApi.ResourceModels.Product>>();

            Assert.NotNull(products);
            Assert.Equal(2, products.Count());
            Assert.Contains(products, p => p.Sku == "TP1");
            Assert.Contains(products, p => p.Sku == "TP2");
            Assert.DoesNotContain(products, p => p.Sku == "AP1");
        });
    }

    [Fact]
    public async Task GetSearchProducts_ReturnsEmptyList_WhenNoProductsMatch()
    {
        await WithTransaction(async () => {
            // Arrange
            CreateNewProduct("test product 1", "TP1", "This is the first product description");
            CreateNewProduct("test product 2", "TP2", "This is the second product description");

            var client = CreateHttpClient();

            // Act
            var response = await client.GetAsync("/api/Search/Products/nomatch");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var products = await response.Content.ReadFromJsonAsync<IEnumerable<EndPointCommerce.WebApi.ResourceModels.Product>>();

            Assert.NotNull(products);
            Assert.Empty(products);
        });
    }

    [Fact]
    public async Task GetSearchSuggestionsProducts_ReturnsMatchingProducts_WhenSearchingByName()
    {
        await WithTransaction(async () => {
            // Arrange
            CreateNewProduct("test product 1", "TP1", "This is the first product description");
            CreateNewProduct("test product 2", "TP2", "This is the second product description");
            CreateNewProduct("another product", "AP1", "This product is unrelated");

            var client = CreateHttpClient();

            // Act
            var response = await client.GetAsync("/api/Search/Suggestions/Products/test");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var results = await response.Content.ReadFromJsonAsync<IEnumerable<Domain.Interfaces.IProductRepository.SearchSuggestion>>();

            Assert.NotNull(results);
            Assert.Equal(3, results.Count());
            Assert.Contains(results, r => r.Term == "test" && r.Count == 2);
            Assert.Contains(results, r => r.Term == "test product 1 (TP1)" && r.Count == 1);
            Assert.Contains(results, r => r.Term == "test product 2 (TP2)" && r.Count == 1);
        });
    }

    [Fact]
    public async Task GetSearchSuggestionsProducts_ReturnsMatchingProducts_WhenSearchingBySku()
    {
        await WithTransaction(async () => {
            // Arrange
            CreateNewProduct("test product 1", "TP1", "This is the first product description");
            CreateNewProduct("test product 2", "TP2", "This is the second product description");
            CreateNewProduct("another product", "AP1", "This product is unrelated");

            var client = CreateHttpClient();

            // Act
            var response = await client.GetAsync("/api/Search/Suggestions/Products/TP1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var results = await response.Content.ReadFromJsonAsync<IEnumerable<Domain.Interfaces.IProductRepository.SearchSuggestion>>();

            Assert.NotNull(results);
            Assert.Equal(2, results.Count());
            Assert.Contains(results, r => r.Term == "TP1" && r.Count == 1);
            Assert.Contains(results, r => r.Term == "test product 1 (TP1)" && r.Count == 1);
        });
    }

    [Fact]
    public async Task GetSearchSuggestionsProducts_ReturnsMatchingProducts_WhenSearchingByDescription()
    {
        await WithTransaction(async () => {
            // Arrange
            CreateNewProduct("test product 1", "TP1", "This is the first product description");
            CreateNewProduct("test product 2", "TP2", "This is the second product description");
            CreateNewProduct("another product", "AP1", "This product is unrelated");

            var client = CreateHttpClient();

            // Act
            var response = await client.GetAsync("/api/Search/Suggestions/Products/description");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var results = await response.Content.ReadFromJsonAsync<IEnumerable<Domain.Interfaces.IProductRepository.SearchSuggestion>>();

            Assert.NotNull(results);
            Assert.Equal(3, results.Count());
            Assert.Contains(results, r => r.Term == "description" && r.Count == 2);
            Assert.Contains(results, r => r.Term == "test product 1 (TP1)" && r.Count == 1);
            Assert.Contains(results, r => r.Term == "test product 2 (TP2)" && r.Count == 1);
        });
    }

    [Fact]
    public async Task GetSearchSuggestionsProducts_ReturnsEmptyList_WhenNoProductsMatch()
    {
        await WithTransaction(async () => {
            // Arrange
            CreateNewProduct("test product 1", "TP1", "This is the first product description");
            CreateNewProduct("test product 2", "TP2", "This is the second product description");

            var client = CreateHttpClient();

            // Act
            var response = await client.GetAsync("/api/Search/Suggestions/Products/nomatch");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var results = await response.Content.ReadFromJsonAsync<IEnumerable<Domain.Interfaces.IProductRepository.SearchSuggestion>>();

            Assert.NotNull(results);
            Assert.Single(results);
            Assert.Contains(results, r => r.Term == "nomatch" && r.Count == 0);
        });
    }
}
