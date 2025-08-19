// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.Net;
using System.Net.Http.Json;
using EndPointEcommerce.Tests.Fixtures;
using EndPointEcommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EndPointEcommerce.Tests.WebApi.Controllers;

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

        var states = await response.Content.ReadFromJsonAsync<IEnumerable<EndPointEcommerce.WebApi.ResourceModels.State>>();

        Assert.NotNull(states);
        Assert.Equal(51, states.Count());
    }
}
