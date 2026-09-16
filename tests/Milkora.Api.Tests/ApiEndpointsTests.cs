using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Milkora.Api.Tests;

/// <summary>
/// End-to-end tests that boot the real API in-memory (TestServer) and hit it over
/// HTTP against the seeded LocalDB (MilkoraDB). Read-only except one create→delete
/// round-trip that cleans up after itself.
/// </summary>
public class ApiEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };

    public ApiEndpointsTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    private sealed record Envelope<T>(bool Success, string? Message, Dictionary<string, string[]>? Errors, T? Data);

    [Fact]
    public async Task GetAnimals_returns_200_with_seed_data()
    {
        var res = await _client.GetAsync("/api/animals");

        res.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await res.Content.ReadFromJsonAsync<Envelope<List<JsonElement>>>(Json);
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull();
        body.Data!.Count.Should().BeGreaterThanOrEqualTo(3); // 3 seed animals
    }

    [Fact]
    public async Task GetDashboard_returns_200()
    {
        var res = await _client.GetAsync("/api/reports/dashboard");

        res.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await res.Content.ReadFromJsonAsync<Envelope<JsonElement>>(Json);
        body!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetAnimalById_unknown_returns_404_envelope()
    {
        var res = await _client.GetAsync($"/api/animals/{Guid.NewGuid()}");

        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await res.Content.ReadFromJsonAsync<Envelope<JsonElement>>(Json);
        body!.Success.Should().BeFalse();
        body.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateAnimal_with_empty_tag_returns_400_with_errors()
    {
        var res = await _client.PostAsJsonAsync("/api/animals", new { tagNumber = "", status = "Milking" });

        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await res.Content.ReadFromJsonAsync<Envelope<JsonElement>>(Json);
        body!.Success.Should().BeFalse();
        body.Errors.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateThenDelete_animal_roundtrip()
    {
        var tag = "IT-" + DateTime.Now.ToString("HHmmssfff");

        // Create
        var createRes = await _client.PostAsJsonAsync("/api/animals",
            new { tagNumber = tag, name = "IntegrationCow", status = "Milking", dailyMilkYield = 5 });
        createRes.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createRes.Content.ReadFromJsonAsync<Envelope<JsonElement>>(Json);
        created!.Success.Should().BeTrue();
        var id = created.Data.GetProperty("animalId").GetGuid();
        id.Should().NotBe(Guid.Empty);

        // Read back
        var getRes = await _client.GetAsync($"/api/animals/{id}");
        getRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // Delete (API returns 200 with a message envelope, not 204)
        var delRes = await _client.DeleteAsync($"/api/animals/{id}");
        delRes.StatusCode.Should().Be(HttpStatusCode.OK);

        // Gone
        var goneRes = await _client.GetAsync($"/api/animals/{id}");
        goneRes.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
