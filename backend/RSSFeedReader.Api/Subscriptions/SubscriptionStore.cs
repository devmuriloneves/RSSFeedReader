namespace RSSFeedReader.Api.Subscriptions;

public sealed record Subscription(string Address);

public sealed class SubscriptionStore
{
    private readonly object _gate = new();
    private readonly List<Subscription> _subscriptions = [];

    public IReadOnlyList<Subscription> GetAll()
    {
        lock (_gate)
        {
            return _subscriptions.ToArray();
        }
    }

    public bool TryAdd(string? address, out Subscription? subscription)
    {
        if (string.IsNullOrEmpty(address))
        {
            subscription = null;
            return false;
        }

        lock (_gate)
        {
            subscription = new Subscription(address);
            _subscriptions.Add(subscription);
            return true;
        }
    }
}