// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.Net;
using System.Net.Http.Json;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Tests.Fixtures;
using EndPointEcommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EndPointEcommerce.Tests.WebApi.Controllers;

public class ProductsControllerTests : IntegrationTests
{
    public ProductsControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture database) :
        base(factory, database) { }

    private Product CreateNewProduct(string name, string sku, bool isEnabled = true, Category? category = null, string? urlKey = null)
    {
        var newProduct = new Product()
        {
            Name = name,
            Sku = sku,
            UrlKey = urlKey,
            BasePrice = 10.00M,
            IsEnabled = isEnabled,
            Category = category
        };

        dbContext.Products.Add(newProduct);
        dbContext.SaveChanges();

        return newProduct;
    }

    private Category CreateNewCategory(string name, string? urlKey = null, bool isEnabled = true)
    {
        var newCategory = new Category()
        {
            Name = name,
            UrlKey = urlKey,
            IsEnabled = isEnabled
        };

        dbContext.Categories.Add(newCategory);
        dbContext.SaveChanges();

        return newCategory;
    }

    [Fact]
    public async Task GetProducts_ReturnsAllEnabledProducts()
    {
        // Arrange
        CreateNewProduct("test_name_1", "test_sku_1");
        CreateNewProduct("test_name_2", "test_sku_2");
        CreateNewProduct("test_name_3", "test_sku_3", false);

        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync("/api/Products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var products = await response.Content.ReadFromJsonAsync<List<EndPointEcommerce.WebApi.ResourceModels.Product>>();

        Assert.NotNull(products);
        Assert.Equal(2, products.Count);
        Assert.Contains(products, p => p.Name == "test_name_1");
        Assert.Contains(products, p => p.Name == "test_name_2");
        Assert.DoesNotContain(products, p => p.Name == "test_name_3");
    }

    [Fact]
    public async Task GetProducts_ReturnsEmptyList_WhenNoEnabledProductsExist()
    {
        // Arrange
        CreateNewProduct("test_name_1", "test_sku_1", false);

        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync("/api/Products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = await response.Content.ReadFromJsonAsync<List<EndPointEcommerce.WebApi.ResourceModels.Product>>();

        Assert.NotNull(products);
        Assert.Empty(products);
    }

    [Fact]
    public async Task GetProductsByCategoryId_ReturnsAllMatchingEnabledProducts()
    {
        // Arrange
        // create a new category
        var category = CreateNewCategory("test_category");

        CreateNewProduct("test_name_1", "test_sku_1", category: category);
        CreateNewProduct("test_name_2", "test_sku_2");
        CreateNewProduct("test_name_3", "test_sku_3", isEnabled: false);

        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync($"/api/Products/CategoryId/{category.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = await response.Content.ReadFromJsonAsync<List<EndPointEcommerce.WebApi.ResourceModels.Product>>();

        Assert.NotNull(products);
        Assert.Single(products);
        Assert.Contains(products, p => p.Name == "test_name_1");
    }

    [Fact]
    public async Task GetProductsByCategoryUrlKey_ReturnsAllMatchingEnabledProducts()
    {
        // Arrange
        var category = CreateNewCategory("test_category", "test_url_key");

        CreateNewProduct("test_name_1", "test_sku_1", category: category);
        CreateNewProduct("test_name_2", "test_sku_2");
        CreateNewProduct("test_name_3", "test_sku_3", isEnabled: false);

        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync($"/api/Products/CategoryUrlKey/{category.UrlKey}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = await response.Content.ReadFromJsonAsync<List<EndPointEcommerce.WebApi.ResourceModels.Product>>();

        Assert.NotNull(products);
        Assert.Single(products);
        Assert.Contains(products, p => p.Name == "test_name_1");
    }

    [Fact]
    public async Task GetProduct_ReturnsTheMatchingProduct()
    {
        // Arrange
        var product = CreateNewProduct("test_name_1", "test_sku_1");

        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync($"/api/Products/{product.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<EndPointEcommerce.WebApi.ResourceModels.Product>();

        Assert.NotNull(result);
        Assert.Contains("test_name_1", result.Name);
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFound_WhenTheMatchingProductIsNotEnabled()
    {
        // Arrange
        var product = CreateNewProduct("test_name_1", "test_sku_1", isEnabled: false);

        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync($"/api/Products/{product.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFound_WhenTheProductDoesNotExist()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync($"/api/Products/{123}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProductByUrlKey_ReturnsTheMatchingProduct()
    {
        // Arrange
        var product = CreateNewProduct("test_name_1", "test_sku_1", urlKey: "test_url_key_1");

        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync($"/api/Products/UrlKey/{product.UrlKey}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<EndPointEcommerce.WebApi.ResourceModels.Product>();

        Assert.NotNull(result);
        Assert.Contains("test_name_1", result.Name);
    }

    [Fact]
    public async Task GetProductByUrlKey_ReturnsNotFound_WhenTheMatchingProductIsNotEnabled()
    {
        // Arrange
        var product = CreateNewProduct("test_name_1", "test_sku_1", urlKey: "test_url_key_1", isEnabled: false);

        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync($"/api/Products/UrlKey/{product.UrlKey}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProductByUrlKey_ReturnsNotFound_WhenTheProductDoesNotExist()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync("/api/Products/UrlKey/not_a_url_key");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
