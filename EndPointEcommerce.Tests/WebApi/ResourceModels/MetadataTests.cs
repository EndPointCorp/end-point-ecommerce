// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.WebApi.ResourceModels;

namespace EndPointEcommerce.Tests.WebApi.ResourceModels;

public class MetadataTests
{
    class TestSeoEntity : BaseSeoEntity
    {
        public string? ANonSeoField { get; set; }
        public int AnotherNonSeoField { get; set; }
    }

    [Fact]
    public void FromEntity_ReturnsANewInstance_WithTheCorrectFields()
    {
        // Arrange
        var testEntityWithMetadata = new TestSeoEntity
        {
            Id = 10,

            UrlKey = "test_url_key",
            MetaTitle = "test_meta_title",
            MetaKeywords = "test_meta_keywords",
            MetaDescription = "test_meta_description",

            ANonSeoField = "test_non_seo_field",
            AnotherNonSeoField = 123
        };

        var result = Metadata.FromEntity(testEntityWithMetadata);

        // Act & Assert
        Assert.Equal("test_meta_title", result.Title);
        Assert.Equal("test_meta_keywords", result.Keywords);
        Assert.Equal("test_meta_description", result.Description);
    }
}
