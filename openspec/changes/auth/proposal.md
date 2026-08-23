## Why

The application needs a consistent authentication and authorization model that fits the existing web application structure, protects command routes, and supports a persistent `User` entity for account and maintenance data. The first phase should keep the design simple: cookie-based sign-in, password hashing, initial `User` / `Admin` roles, and user maintenance through a thin page shell with JavaScript-driven query and command calls, plus shared client auth/session events such as app.signout, using validate-on-request as the cookie safety net and immediate server-pushed sign-out when auth-sensitive changes require it. The in-memory unit of work includes a bootstrap `Admin` user in `UnitOfWorkInMemory.cs` so the User Maintenance screen can be reached on first run. Custom claims are reserved for future authorization needs, and password reset and MFA remain out of scope for the first phase.

## What Changes

- Add sign-in, sign-out, and change-password flows.
- Store user account data in a persistent `User` entity.
- Persist password hashes, roles, and user status in the application data layer.
- Use ASP.NET Core cookie authentication for browser sessions.
- Protect command middleware with authentication and roles.
- Keep query middleware public for now, with room to secure it later.
- Add a thin user-maintenance page shell that uses JavaScript-driven query and command calls following the same Contacts-style interaction pattern.
- Seed the in-memory unit of work with a bootstrap `Admin` user so the maintenance screen is reachable on first run.
- Surface auth/session outcomes to the browser through shared client events such as app.signout so the UI can react before redirecting when needed.
- Use validate-on-request for cookie safety and immediate server-pushed sign-out when the current user is affected by maintenance changes.
- Allow custom claims beyond the initial role set so authorization can grow without redesigning the account model.
- Avoid password reset and MFA in the first phase.

## Capabilities

### New Capabilities
- `auth`: authentication/authorization with persistent user storage, cookie sessions, and command protection.

### Modified Capabilities
- `<existing-name>`: none

## Impact

- Adds authentication and authorization services to the web application.
- Introduces a persistent `User` entity and supporting user-maintenance flow.
- Protects command middleware with authenticated principals and roles.
- Keeps query middleware open initially, with a future path for query authorization.
- Requires new account and user-maintenance shell, routes, views, JavaScript interactions, and shared auth/session client events with validate-on-request cookie safety and future custom-claims extensibility.
