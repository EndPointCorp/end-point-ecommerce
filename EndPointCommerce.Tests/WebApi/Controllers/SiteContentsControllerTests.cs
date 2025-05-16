using System.Net;
using System.Net.Http.Json;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Tests.Fixtures;
using EndPointCommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Tests.WebApi.Controllers;

public class SiteContentsControllerTests : IntegrationTests
{
    public SiteContentsControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture database) :
        base(factory, database) { }

    private SiteContent CreateNewSiteContent(string name, string content)
    {
        var newSiteContent = new SiteContent() {
            Name = name,
            Content = content
        };

        dbContext.SiteContents.Add(newSiteContent);
        dbContext.SaveChanges();

        return newSiteContent;
    }

    [Fact]
    public async Task GetSiteContents_ReturnsAllSiteContents_OrderedByName()
    {
        // Arrange
        dbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE site_contents");

        CreateNewSiteContent("test_name_1", "test_content_1");
        CreateNewSiteContent("test_name_2", "test_content_2");
        CreateNewSiteContent("test_name_3", "test_content_3");

        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync("/api/SiteContents");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var siteContents = await response.Content.ReadFromJsonAsync<List<SiteContent>>();

        Assert.NotNull(siteContents);
        Assert.Equal(3, siteContents.Count);
        Assert.Equal("test_name_1", siteContents[0].Name);
        Assert.Equal("test_content_1", siteContents[0].Content);
        Assert.Equal("test_name_2", siteContents[1].Name);
        Assert.Equal("test_content_2", siteContents[1].Content);
        Assert.Equal("test_name_3", siteContents[2].Name);
        Assert.Equal("test_content_3", siteContents[2].Content);
    }
}
