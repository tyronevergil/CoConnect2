## Why

The application needs a consistent authentication and authorization model that fits the existing MVC structure, protects command routes, and supports a persistent `User` entity for account and maintenance data. The first phase should keep the design simple: cookie-based sign-in, password hashing, roles, and user maintenance through the persistence layer, without adding password reset or MFA yet.

## What Changes

- Add MVC-based login, logout, and change-password flows.
- Store user account data in a persistent `User` entity.
- Persist password hashes, roles, and user status in the application data layer.
- Use ASP.NET Core cookie authentication for browser sessions.
- Protect command middleware with authentication and roles.
- Keep query middleware public for now, with room to secure it later.
- Add a `UsersController` that follows the same CQRS-style flow as Contacts for user maintenance.
- Avoid password reset and MFA in the first phase.

## Capabilities

### New Capabilities
- `auth`: MVC-based authentication/authorization with persistent user storage, cookie sessions, and command protection.

### Modified Capabilities
- `<existing-name>`: none

## Impact

- Adds authentication and authorization services to the MVC web application.
- Introduces a persistent `User` entity and supporting user-maintenance flow.
- Protects command middleware with authenticated principals and roles.
- Keeps query middleware open initially, with a future path for query authorization.
- Requires new MVC account and user-maintenance routes and views.
