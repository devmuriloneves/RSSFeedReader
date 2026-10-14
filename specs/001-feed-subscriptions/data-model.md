# Data Model: MVP RSS Feed Subscriptions

## Subscription

Represents one address submitted by the user for the current backend process.

| Field     | Type   | Rules                                                                                                                                                              |
| --------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `address` | string | Required and must contain at least one non-whitespace character. Preserve exactly as submitted. It is display text only and is never interpreted as a URL or feed. |

### Lifecycle and relationships

- A successful add appends a new subscription to the process-local subscription list.
- Duplicate addresses are separate entries; no identifier or de-duplication behavior is needed for the MVP.
- Entries are returned in insertion order.
- The list is cleared when the backend process stops; there is no persistence or user partitioning.
- A missing, null, empty, or whitespace-only address is rejected and does not change the list.

## Subscription List

An ordered collection of `Subscription` values held by one backend process. `GET` returns a snapshot of this collection; `POST` adds exactly one value. Concurrent requests must not corrupt the collection.
