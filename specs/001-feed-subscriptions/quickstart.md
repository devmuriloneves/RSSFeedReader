# Quickstart: Subscription MVP Validation

## Prerequisites

- .NET 10 SDK
- A current browser
- The solution and project files described in [plan.md](plan.md)

## Build and automated API tests

From the repository root:

```sh
dotnet restore RSSFeedReader.sln
dotnet build RSSFeedReader.sln --no-restore
dotnet test RSSFeedReader.sln --no-build
```

Expected: restore and build succeed; API tests verify empty initial state, successful add/list, exact address preservation, duplicate retention, rejection of empty input without mutation, and a fresh host starting empty.

## Run the API and smoke-test its contract

In one terminal:

```sh
dotnet run --project backend/RSSFeedReader.Api
```

The API listens at `http://localhost:5151` as configured for local development. In another terminal:

```sh
curl -i http://localhost:5151/api/subscriptions
curl -i -H 'Content-Type: application/json' \
  -d '{"address":"https://example.org/feed.xml"}' \
  http://localhost:5151/api/subscriptions
curl -i http://localhost:5151/api/subscriptions
curl -i -H 'Content-Type: application/json' \
  -d '{"address":""}' \
  http://localhost:5151/api/subscriptions
curl -i -H 'Content-Type: application/json' \
  -d '{"address":"   "}' \
  http://localhost:5151/api/subscriptions
```

Expected: the first list is `[]`; the add returns `200` with the same address; the next list contains it; empty and whitespace-only addresses return `400` and leave the list unchanged.

## Validate the browser flow

In a second terminal, run the Blazor app:

```sh
dotnet run --project frontend/RSSFeedReader.UI
```

Open the printed frontend URL. Add two different addresses and verify both appear unchanged in insertion order. Submit an empty or whitespace-only value and verify the list is unchanged. Confirm the browser console has no API or CORS errors.

Acceptance checklist:

- [ ] The subscriptions page loads without route or browser-console errors.
- [ ] Adding two different addresses displays both exactly as entered and in insertion order.
- [ ] Submitting an empty or whitespace-only value leaves the displayed list unchanged.
- [ ] Restarting the backend and reloading the UI shows an empty list.

Stop and restart the backend process, then reload the UI. Expected: the list is empty. This verifies the documented backend-process session boundary; closing only the browser while leaving the API running does not clear backend memory.

## Configuration checks

- `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` points to `http://localhost:5151/api/`.
- Backend CORS allows the actual frontend development origin from its launch settings.
- Only the subscription page uses the `/` route; remove template Home, Counter, and Weather pages and their navigation links before adding it.

For request and response schemas, see [contracts/openapi.yaml](contracts/openapi.yaml). For field and lifecycle rules, see [data-model.md](data-model.md).
