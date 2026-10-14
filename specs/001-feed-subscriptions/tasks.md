# Tasks: RSS Feed Subscriptions

**Input**: Design documents from `/specs/001-feed-subscriptions/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Initialize the solution and app structure for the MVP.

- [x] T001 Create the project structure under `backend/` and `frontend/` and add the solution entry at `RSSFeedReader.sln`
- [x] T002 Initialize the .NET 10 ASP.NET Core API and Blazor WebAssembly app in `backend/RSSFeedReader.Api/RSSFeedReader.Api.csproj` and `frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj`
- [x] T003 [P] Configure the local API base URL and CORS settings in `frontend/RSSFeedReader.UI/wwwroot/appsettings.json` and `backend/RSSFeedReader.Api/Program.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish the backend state and API contract before any UI work can rely on it.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [x] T004 Implement the singleton in-memory subscription store in `backend/RSSFeedReader.Api/Subscriptions/SubscriptionStore.cs` with process-local list behavior, insertion-order retention, and empty-value rejection
- [x] T005 Implement the `GET /api/subscriptions` and `POST /api/subscriptions` endpoints in `backend/RSSFeedReader.Api/Program.cs` using the contract in `specs/001-feed-subscriptions/contracts/openapi.yaml`
- [x] T006 [P] Scaffold the API test project and add regression coverage in `backend/RSSFeedReader.Api.Tests/SubscriptionsApiTests.cs` for empty state, add/list flow, duplicates, and empty rejection

**Checkpoint**: The backend contract is functional and the in-memory lifecycle is confirmed before UI implementation starts.

---

## Phase 3: User Story 1 - Add and view subscriptions (Priority: P1)

**Goal**: Let a user enter a feed address, add it to the current session's list, and immediately see it in the UI without validating or fetching the feed.

**Independent Test**: Start from an empty list, add one address, and verify the same address appears immediately; add a second address and verify both are visible; submit an empty value and verify the list remains unchanged.

### Tests for User Story 1

- [x] T007 [P] [US1] Add API tests in `backend/RSSFeedReader.Api.Tests/SubscriptionsApiTests.cs` for the exact response behavior in `specs/001-feed-subscriptions/contracts/openapi.yaml`
- [x] T008 [P] [US1] Add the manual browser validation checklist in `specs/001-feed-subscriptions/quickstart.md` for the add/list flow and the empty-input rejection case

### Implementation for User Story 1

- [x] T009 [US1] Complete the backend validation and response contract in `backend/RSSFeedReader.Api/Program.cs` so a non-empty `address` is stored verbatim and an empty/missing value returns `400`
- [x] T010 [P] [US1] Implement the frontend API client in `frontend/RSSFeedReader.UI/Services/SubscriptionsApiClient.cs` for `GET` and `POST` requests to `/api/subscriptions`
- [x] T011 [US1] Build the subscription form and list rendering in `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor` so the user enters text, submits it, and sees the list update immediately
- [x] T012 [US1] Update the UI startup configuration in `frontend/RSSFeedReader.UI/Program.cs` to expose the subscription page and remove the default template navigation/pages
- [x] T013 [P] [US1] Verify exact text preservation and insertion order across the API and UI in `backend/RSSFeedReader.Api/Subscriptions/SubscriptionStore.cs`, `backend/RSSFeedReader.Api/Program.cs`, and `frontend/RSSFeedReader.UI/Pages/Subscriptions.razor`

**Checkpoint**: User Story 1 is complete and independently testable as a working add/list MVP in a single process session.

---

## Phase 4: Polish & Cross-Cutting Concerns

**Purpose**: Final verification, cleanup, and MVP readiness checks.

- [x] T014 [P] Run the end-to-end validation steps in `specs/001-feed-subscriptions/quickstart.md` and fix API, CORS, or route issues discovered during local execution
- [x] T015 [P] Confirm the implementation remains within the MVP scope described in `specs/001-feed-subscriptions/spec.md`, `research.md`, and `data-model.md` by checking that no feed fetching, validation, persistence, or remove flows were added

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; can start immediately.
- **Foundational (Phase 2)**: Depends on Setup completion and blocks User Story 1.
- **User Story 1 (Phase 3)**: Depends on the foundational API contract and store.
- **Polish (Phase 4)**: Depends on Story 1 completion and validation.

### User Story Dependencies

- **User Story 1 (P1)**: No dependency on other user stories; this is the only story in the MVP.

### Within Each User Story

- Tests are written first and must describe the expected add/list behavior before implementation.
- Backend store and API endpoints execute before the frontend page logic.
- UI page behavior is validated after the API contract is working.

### Parallel Opportunities

- Setup tasks T001-T003 can run in parallel when the team is ready to create the solution and app scaffolding.
- Foundational tasks T004-T006 can be split across backend and test work, with the test project and store logic developing independently.
- User Story 1 testing tasks T007-T008 can be completed together while implementation tasks T009-T013 proceed in parallel by area (backend, frontend, verification).
- Final validation and scope checks T014-T015 can run together after the MVP is complete.

---

## Parallel Example: User Story 1

```bash
# Backend and tests in parallel
# 1) Implement store + API contract
# 2) Add regression tests for add/list and empty rejection

# Frontend in parallel
# 1) Add API client service
# 2) Build resulting subscription page and route configuration
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational backend contract and in-memory store.
3. Complete Phase 3: User Story 1 add/list flow.
4. Stop and validate the browser flow and API contract.
5. Ship the MVP only if the user can add and see subscriptions as text without feed processing.

### Incremental Delivery

1. Create the solution, backend, and frontend projects.
2. Implement the in-memory store and API endpoints.
3. Add the UI page and API client.
4. Validate the session-bounded behavior and remove any template UI or extra scope.
5. Keep the release limited to add/list behavior; no feed fetching or persistence is added.

---

## Notes

- [P] tasks are allowed only when they touch different files or areas with no dependency on incomplete work.
- [US1] labels map directly to the user story in `specs/001-feed-subscriptions/spec.md`.
- Each task includes a concrete file path to keep implementation narrow and reviewable.
- The feature must stay within the documented MVP scope: add one or more non-empty addresses, show them exactly as entered, and keep them only for the current backend process.
