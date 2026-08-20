- [x] Create a shared `QueryRouting` project
  - Define `QueryRouteAttribute` and `QueryRouteResultKind` in a dependency-light project referenced by both `Persistence` and the web app.
  - Keep the shared contract free of middleware or persistence runtime logic.

- [x] Add query route metadata to specifications
  - Introduce the shared route metadata on query specification factory methods.
  - Annotate existing contact query spec methods so they can be discovered by the middleware.

- [x] Build a query route registry in the web layer
  - Scan the shared contract assembly at startup for query metadata.
  - Register routes with verb, template, cardinality, and parameter binding details.
  - Validate duplicates and malformed route definitions early.

- [x] Create a query execution helper in the web layer
  - Resolve `IQueryContextFactory` and execute matched specifications through a shared helper.
  - Support collection and single-item result handling.
  - Normalize `404 Not Found` behavior for single-item misses.

- [x] Add query middleware
  - Intercept supported `GET /api/query/...` requests.
  - Match incoming requests to the registry and execute the corresponding query.
  - Write JSON responses directly and fall through for unmatched routes.

- [x] Wire the middleware into the ASP.NET pipeline
  - Register the middleware in `Program.cs` in the correct order.
  - Route supported query requests through middleware only.

- [x] Remove `QueryContactController.cs`
  - Delete the controller after middleware is confirmed to handle the supported query endpoints.

- [x] Validate behavior
  - Verify the middleware returns the expected contact query results, including empty collections.
  - Confirm query contexts remain no-tracking and build succeeds.
