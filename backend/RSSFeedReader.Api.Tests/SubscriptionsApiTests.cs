using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using RSSFeedReader.Api.Subscriptions;

namespace RSSFeedReader.Api.Tests;

public sealed class SubscriptionsApiTests
{
    [Fact]
    public async Task Get_returns_empty_list_for_a_new_backend_process()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var subscriptions = await client.GetFromJsonAsync<Subscription[]>("/api/subscriptions");

        Assert.NotNull(subscriptions);
        Assert.Empty(subscriptions);
    }

    [Fact]
    public async Task Post_returns_subscription_and_get_lists_it()
    {
        const string address = "https://example.org/feed.xml";
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync("/api/subscriptions", new { address });
        var created = await response.Content.ReadFromJsonAsync<Subscription>();
        var subscriptions = await client.GetFromJsonAsync<Subscription[]>("/api/subscriptions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(address, created?.Address);
        Assert.NotNull(subscriptions);
        Assert.Equal([address], subscriptions.Select(subscription => subscription.Address));
    }

    [Fact]
    public async Task Post_preserves_exact_text_duplicates_and_insertion_order()
    {
        const string address = "  not validated as a URL  ";
        const string secondAddress = "https://example.org/second.xml";
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/subscriptions", new { address });
        await client.PostAsJsonAsync("/api/subscriptions", new { address = secondAddress });
        await client.PostAsJsonAsync("/api/subscriptions", new { address });
        var subscriptions = await client.GetFromJsonAsync<Subscription[]>("/api/subscriptions");

        Assert.NotNull(subscriptions);
        Assert.Equal(
            [address, secondAddress, address],
            subscriptions.Select(subscription => subscription.Address));
    }

    [Fact]
    public async Task Post_rejects_empty_address_without_mutating_list()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync("/api/subscriptions", new { address = string.Empty });
        var subscriptions = await client.GetFromJsonAsync<Subscription[]>("/api/subscriptions");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(subscriptions);
        Assert.Empty(subscriptions);
    }

    [Fact]
    public async Task Post_rejects_missing_address_without_mutating_list()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync("/api/subscriptions", new { });
        var subscriptions = await client.GetFromJsonAsync<Subscription[]>("/api/subscriptions");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(subscriptions);
        Assert.Empty(subscriptions);
    }

    [Fact]
    public async Task Post_rejects_malformed_json()
    {
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();
        using var content = new StringContent("{", Encoding.UTF8, "application/json");

        using var response = await client.PostAsync("/api/subscriptions", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task New_backend_process_starts_with_an_empty_list()
    {
        using var firstFactory = new WebApplicationFactory<Program>();
        using var firstClient = firstFactory.CreateClient();
        await firstClient.PostAsJsonAsync("/api/subscriptions", new { address = "https://example.org/feed.xml" });

        using var secondFactory = new WebApplicationFactory<Program>();
        using var secondClient = secondFactory.CreateClient();
        var subscriptions = await secondClient.GetFromJsonAsync<Subscription[]>("/api/subscriptions");

        Assert.NotNull(subscriptions);
        Assert.Empty(subscriptions);
    }
}