# Research: MVP RSS Feed Subscriptions

## Decisions

### API ownership and shape

- **Decision**: Keep subscription state in the backend and expose `GET /api/subscriptions` and `POST /api/subscriptions`.
- **Rationale**: The stakeholder architecture assigns in-memory storage and add/list operations to the API, with user interaction in Blazor. Separate operations make the UI and API responsibilities explicit and testable.
- **Alternatives considered**: Browser-only state would not exercise the required API boundary; a combined read/write endpoint would make the contract less clear.

### In-memory lifecycle

- **Decision**: Use one backend-process-scoped store, accessed through a singleton service. Treat the app session as the lifetime of that local backend process.
- **Rationale**: A singleton preserves state across API requests without adding persistence. Stopping and restarting the backend produces an empty list, matching the documented in-memory behavior.
- **Alternatives considered**: Per-request state would lose subscriptions between calls; a database or browser storage would add persistence outside MVP scope. Closing only a browser tab does not stop a separately running development API, so session-reset validation must restart the API.

### Address behavior

- **Decision**: Reject null or empty address values; otherwise preserve the submitted string exactly and append every submission, including duplicates. Do not trim, parse, validate, fetch, or navigate to it.
- **Rationale**: This implements the spec's non-empty requirement and exact-display rule while keeping user input inert. Normal Razor text rendering provides output encoding.
- **Alternatives considered**: URL validation, normalization, and de-duplication are not required and could change the text the user entered.

### Runtime and project layout

- **Decision**: Target .NET 10 (`net10.0`) with separate ASP.NET Core API and Blazor WebAssembly projects under the stakeholder-documented `backend/` and `frontend/` paths.
- **Rationale**: The stakeholder documents select this stack and layout. .NET SDK 10.0.401 is installed in the planning environment, so .NET 10 is available as the initial target.
- **Alternatives considered**: Older target frameworks are not prescribed and would not improve this MVP; a single-project design would depart from the documented API/UI separation.

### Verification approach

- **Decision**: Use xUnit with `Microsoft.AspNetCore.Mvc.Testing` for API behavior and HTTP contract tests; validate the Blazor flow with the documented local browser acceptance scenario.
- **Rationale**: No test framework or project exists in the workspace. These tests cover the API state and response behavior without introducing feed libraries or production infrastructure.
- **Alternatives considered**: Manual-only API testing would not give repeatable regression coverage; a separate frontend test framework is unnecessary for this minimal UI before an established convention exists.

## Findings

- The feature has no existing source, solution, or test layout to extend; the proposed structure is greenfield.
- ProjectGoals.md and AppFeatures.md both restrict MVP behavior to adding and listing subscriptions, with in-memory storage and no feed operations.
- TechStack.md specifies ASP.NET Core Web API + Blazor WebAssembly and asks for explicit API URL/CORS configuration.
- The spec uses “app session” for the period in which subscriptions are available. For this two-process local architecture, the chosen testable boundary is the backend process lifetime.
