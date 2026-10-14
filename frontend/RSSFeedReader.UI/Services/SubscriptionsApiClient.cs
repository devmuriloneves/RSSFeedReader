using System.Net.Http.Json;

namespace RSSFeedReader.UI.Services;

public sealed class SubscriptionsApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<SubscriptionDto>> GetSubscriptionsAsync(
        CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<SubscriptionDto[]>("subscriptions", cancellationToken) ?? [];
    }

    public async Task<SubscriptionDto> AddSubscriptionAsync(
        string address,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "subscriptions",
            new CreateSubscriptionRequest(address),
            cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<SubscriptionDto>(cancellationToken)
            ?? throw new InvalidOperationException("The subscriptions API returned an empty response.");
    }

    private sealed record CreateSubscriptionRequest(string Address);
}

public sealed record SubscriptionDto(string Address);