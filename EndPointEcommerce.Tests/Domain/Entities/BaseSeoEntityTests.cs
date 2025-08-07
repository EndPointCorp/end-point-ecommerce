using System.ComponentModel.DataAnnotations;
using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Tests.Domain.Entities;

public abstract class BaseSeoEntitiesTests
{
    protected abstract BaseSeoEntity BuildSubjectWithUrlKey(string urlKey);

    [Theory]
    [InlineData(" ")]
    [InlineData("`")]
    [InlineData("!")]
    [InlineData("@")]
    [InlineData("#")]
    [InlineData("$")]
    [InlineData("%")]
    [InlineData("^")]
    [InlineData("&")]
    [InlineData("*")]
    [InlineData("+")]
    [InlineData("=")]
    [InlineData("(")]
    [InlineData(")")]
    [InlineData("{")]
    [InlineData("}")]
    [InlineData("[")]
    [InlineData("]")]
    [InlineData("<")]
    [InlineData(">")]
    [InlineData("\\")]
    [InlineData("|")]
    [InlineData("/")]
    [InlineData("?")]
    [InlineData(",")]
    public void Validation_Fails_WhenTheUrlKeyContainsNonUrlSafeCharacters(string urlKey)
    {
        // Arrange
        var subject = BuildSubjectWithUrlKey(urlKey);
        var context = new ValidationContext(subject);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(subject, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
        Assert.Single(results);
        Assert.Equal("Only URI-safe characters are allowed.", results.First().ErrorMessage);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("123")]
    [InlineData("~")]
    [InlineData(".")]
    [InlineData("-")]
    [InlineData("_")]
    public void Validation_Succeeds_WhenTheUrlKeyContainsOnlyUrlSafeCharacters(string urlKey)
    {
        // Arrange
        var subject = BuildSubjectWithUrlKey(urlKey);
        var context = new ValidationContext(subject);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(subject, context, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }
}
