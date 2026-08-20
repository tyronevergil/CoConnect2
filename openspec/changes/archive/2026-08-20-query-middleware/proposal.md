## Why

The application currently exposes read operations through `QueryContactController`, while command processing already uses middleware. Introducing a query middleware would provide a consistent request pipeline for query endpoints and allow new query APIs to be added from specifications with less routing duplication.

## What Changes

- Add a query middleware that intercepts supported `GET /api/query/...` routes.
- Map each query route to a specification factory method, such as `ContactSpecs.GetAll()` and `ContactSpecs.Get(id)`.
- Execute query specifications through `IQueryContextFactory` so read operations remain no-tracking.
- Establish a route-based discovery model so new query specifications can become new query APIs with minimal registration overhead.

## Capabilities

### New Capabilities
- `query-middleware`: route-driven middleware that resolves query requests to specification-backed query handlers.
- `query-route-discovery`: automatic discovery of query routes from specification metadata so new specs can surface as new query APIs.

### Modified Capabilities
- `<existing-name>`: none

## Impact

- Adds a new middleware component in the web application pipeline.
- Introduces a small shared `QueryRouting` project that both `Persistence` and the web app can reference without circular dependencies.
- Keeps the existing `CrudDatastore` specification pattern and `QueryContextFactory` as the execution source for query behavior.
- Uses middleware as the sole path for supported query endpoints.
- Removes the controller-based query endpoint for contacts once middleware is the active path.
