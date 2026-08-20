## Context

The application currently exposes contact read operations through `QueryContactController`, while command requests already use middleware. Query execution is backed by the `CrudDatastore` specification pattern (`ContactSpecs`) and by `QueryContextFactory`, which should keep query-side operations read-only.

To avoid circular project references, the route metadata contract for query endpoints must live in a small shared `QueryRouting` project that both `Persistence` and the web app can reference. The web layer should own discovery, routing, and execution orchestration; `Persistence` should remain focused on specifications and context factories.

## Goals / Non-Goals

**Goals:**
- Add a middleware-based query pipeline for `GET /api/query/...` requests.
- Discover query endpoints from route metadata attached to specification factory methods.
- Host route metadata in a shared contract assembly to avoid circular references.
- Support both collection and single-item query results.
- Execute all query requests through `IQueryContextFactory`.
- Remove the controller-based query path for supported routes.

**Non-Goals:**
- Replace all controllers in the application.
- Build a general-purpose query DSL or ad hoc reflection-based route inference.
- Change the existing specification pattern semantics in `CrudDatastore`.

## Decisions

1. Place query route metadata in a shared `QueryRouting` project referenced by both `Persistence` and the web app.
   - Rationale: the shared contract avoids a circular reference while keeping the route metadata close to the specifications that use it.
   - Alternatives considered: define metadata in `Persistence` (creates coupling to the web layer) or in the web project (creates a reverse dependency from `Persistence` to the web app).

2. Use route metadata on specification factory methods rather than convention-based discovery.
   - Rationale: explicit metadata is easier to reason about and avoids fragile method-name inference.
   - Alternatives considered: hard-coded route map, naming conventions, reflection over spec types. Hard-coded maps are simpler but do not auto-expand; conventions are less reliable.

3. Register discovered routes at startup into a small in-memory route registry in the web layer.
   - Rationale: startup scanning keeps request-time handling fast and centralizes validation of duplicate or malformed routes.
   - Alternatives considered: scan on every request, or rely on ASP.NET endpoint routing directly. Those options add complexity or require deeper framework integration.

4. Remove the controller-based query path for supported routes.
   - Rationale: middleware becomes the single source of truth for supported query routes, which avoids duplicate behavior and reduces routing ambiguity.
   - Alternatives considered: keep the controller as a fallback. That would preserve a second code path and increase maintenance overhead.

5. Delegate query execution to a small helper/service that uses `IQueryContextFactory`.
   - Rationale: middleware should focus on interception and response handling, not query execution details.
   - Alternatives considered: embed query execution directly in middleware. That would make the middleware harder to test and extend.

## Risks / Trade-offs

- [Risk] Metadata becomes part of the API contract → Mitigation: keep metadata minimal and explicit so changes are intentional.
- [Risk] Startup scanning could register duplicate or invalid routes → Mitigation: validate route templates during discovery and fail fast.
- [Risk] Middleware could diverge from the expected contact query semantics during transition → Mitigation: validate the middleware against the expected contact query behavior before deleting the controller.
- [Trade-off] Route metadata couples query specification methods to HTTP concerns → Mitigation: limit metadata to route, verb, and cardinality only.
- [Risk] Shared `QueryRouting` project adds a small amount of solution complexity → Mitigation: keep it metadata-only and dependency-free apart from common abstractions.
