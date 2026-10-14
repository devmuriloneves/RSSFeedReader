# Implementation Plan: MVP RSS Feed Subscriptions

**Branch**: `001-feed-subscriptions` (feature context; current checkout is `main`) | **Date**: 2026-10-13 | **Spec**: [spec.md](spec.md)

**Input**: [ProjectGoals.md](../../StakeholderDocuments/ProjectGoals.md), [TechStack.md](../../StakeholderDocuments/TechStack.md), and [spec.md](spec.md)

## Summary

Deliver a single-user RSS/Atom subscription list using an ASP.NET Core Web API and Blazor WebAssembly. The API accepts and lists address strings using process-local memory; the UI submits addresses and displays the returned list. Empty values are rejected, but addresses are not validated, fetched, parsed, or navigated to. See [research.md](research.md) for key decisions.

## Technical Context

**Language/Version**: C# / .NET 10 (`net10.0`; SDK 10.0.401 is installed)

**Primary Dependencies**: ASP.NET Core Web API, Blazor WebAssembly, `Microsoft.AspNetCore.Mvc.Testing` for API integration tests

**Storage**: In-memory singleton subscription store; state lasts for the backend process and is lost when it stops

**Testing**: xUnit API tests, including HTTP contract coverage with `WebApplicationFactory`; manual browser acceptance for the Blazor flow

**Target Platform**: Windows, macOS, and Linux; browser client runs as WebAssembly

**Project Type**: Web application (API backend and Blazor WebAssembly frontend)

**Performance Goals**: A successful add appears in the visible list within 1 second; single local user and a small subscription list

**Constraints**: No URL validation, feed fetching or parsing, persistence, remove action, or background work; retain entered address text unchanged; configure API URL and CORS explicitly

**Scale/Scope**: One local user; one add/list view and two API operations

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-checked after Phase 1 design._

- **Security and Input Safety**: PASS. Reject only missing/empty address values; treat addresses as text, render with normal Razor text encoding, and never interpret, fetch, or render them as HTML.
- **Maintainable Architecture**: PASS. Keep API state/operations, frontend interaction, and their explicit JSON contract in separate responsibilities.
- **Quality-Driven Delivery**: PASS. Verify API behavior with focused automated tests and the UI with acceptance steps; document configuration and startup assumptions.
- **Incremental MVP Discipline**: PASS. In-memory add/list only; feed operations and persistence remain out of scope.
- **Operational Readiness and Future Extensibility**: PASS. Use explicit API base URL and CORS configuration and document cross-platform run commands.

No constitution violations require justification.

## Project Structure

### Documentation (this feature)

```text
specs/001-feed-subscriptions/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── openapi.yaml
└── tasks.md
```

### Source Code (repository root)

```text
RSSFeedReader.sln
backend/
├── RSSFeedReader.Api/
│   ├── Program.cs
│   └── Subscriptions/
│       └── SubscriptionStore.cs
└── RSSFeedReader.Api.Tests/
    └── SubscriptionsApiTests.cs
frontend/
└── RSSFeedReader.UI/
    ├── Program.cs
    ├── Pages/
    │   └── Subscriptions.razor
    ├── Services/
    │   └── SubscriptionsApiClient.cs
    └── wwwroot/
        └── appsettings.json
```

**Structure Decision**: Use the separate `backend/RSSFeedReader.Api` and `frontend/RSSFeedReader.UI` projects named in the stakeholder development guidance, plus a focused API test project. The workspace currently contains no solution, source, or test projects, so these are proposed paths rather than existing directories. Remove unused Blazor template demo routes/navigation as part of project setup before adding the root subscription page.

## Complexity Tracking

No constitution violations.
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
