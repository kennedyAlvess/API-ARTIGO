using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ApiBenchmark.Tests;

public sealed class ApiEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_returns_ok()
    {
        var response = await _client.GetAsync("/health", TestCancellation);
        var body = await response.Content.ReadFromJsonAsync<HealthResponse>(TestCancellation);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("ok", body.Status);
    }

    [Fact]
    public async Task Product_search_returns_known_product()
    {
        var response = await _client.GetAsync("/api/products?term=cad", TestCancellation);
        var body = await response.Content.ReadFromJsonAsync<ProductSearchResponse>(TestCancellation);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        var product = Assert.Single(body.Items);
        Assert.Equal(1, product.Id);
        Assert.Equal("Caderno", product.Name);
        Assert.Equal("Produto demonstrativo para anotações", product.Description);
    }

    [Theory]
    [InlineData("/api/products")]
    [InlineData("/api/products?term=")]
    [InlineData("/api/products?term=nao-existe")]
    public async Task Empty_or_unmatched_search_returns_empty_collection(string path)
    {
        var response = await _client.GetAsync(path, TestCancellation);
        var body = await response.Content.ReadFromJsonAsync<ProductSearchResponse>(TestCancellation);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Empty(body.Items);
    }

    [Fact]
    public async Task Public_fixture_returns_only_synthetic_content()
    {
        var response = await _client.GetAsync("/api/files?name=public.txt", TestCancellation);
        var content = await response.Content.ReadAsStringAsync(TestCancellation);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Fixture pública sintética do benchmark.", content.Trim());
    }

    [Fact]
    public async Task ApiSc02_controlled_traversal_reaches_only_marker_fixture()
    {
        var response = await _client.GetAsync(
            "/api/files?name=..%2FTestFixtures%2Fmarker.txt",
            TestCancellation);
        var content = await response.Content.ReadAsStringAsync(TestCancellation);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "Marcador sintético: travessia controlada do benchmark API-SC-02.",
            content.Trim());
    }

    [Fact]
    public async Task File_outside_project_is_rejected()
    {
        var response = await _client.GetAsync(
            "/api/files?name=..%2F..%2F..%2Foutside.txt",
            TestCancellation);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private sealed record HealthResponse(string Status);

    private sealed record ProductResponse(int Id, string Name, string Description);

    private sealed record ProductSearchResponse(ProductResponse[] Items);

    private static CancellationToken TestCancellation => TestContext.Current.CancellationToken;
}
