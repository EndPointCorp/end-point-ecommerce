using System.Net;
using System.Net.Http.Json;
using EndPointCommerce.Tests.Fixtures;
using EndPointCommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EndPointCommerce.Tests.WebApi.Controllers;

public class StatesControllerTests : IntegrationTests
{
    public StatesControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture database) :
        base(factory, database) { }

    [Fact]
    public async Task GetStates_ReturnsListOfStates()
    {
        // Arrange
        var client = CreateHttpClient();

        // Act
        var response = await client.GetAsync("/api/States");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var states = await response.Content.ReadFromJsonAsync<IEnumerable<EndPointCommerce.WebApi.ResourceModels.State>>();

        Assert.NotNull(states);
        Assert.Equal(51, states.Count());
    }
}
