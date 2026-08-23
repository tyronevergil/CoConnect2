## Context

The authentication work must fit this existing structure and reuse the same persistence-first approach for user account data while following the existing thin page-shell and JavaScript-driven query/command interaction pattern. Auth-sensitive flows should use validate-on-request as the cookie safety net and immediate server-pushed sign-out events when the current user is affected, while still allowing authorization to evolve beyond the initial role set through custom claims.

## Goals / Non-Goals

**Goals:**
- Add sign-in, sign-out, and change-password flows.
- Persist a separate `User` entity for account-maintenance data.
- Use ASP.NET Core cookie authentication for browser sessions.
- Secure command middleware with authenticated principals and roles.
- Keep query middleware public initially, with a clear path to secure it later.
- Model user maintenance as a thin page shell with JavaScript-driven query and command calls rather than a dedicated CRUD page or controller.
- Capture the `User` entity fields, role enum, and user query/command route contract in the spec.
- Capture the UI flow for login, change password, user maintenance, shared layout/navigation, and shared auth/session client events such as `app.signout`.
- Leave room for custom claims beyond the initial role set so the authorization model can evolve without redesigning the user account shape.

**Non-Goals:**
- Add password reset or MFA in the first phase.
- Replace the current page model structure with an incompatible UI framework rewrite.
- Move contacts onto the new auth flow.
- Introduce external identity providers in this phase.

## Decisions

1. Use thin page shells with JavaScript driving query and command interactions.
   - Rationale: the existing Contacts page shows that the shell can stay thin while the browser calls query and command endpoints directly.
   - Alternatives considered: a controller-centric CRUD flow or a page-model-specific rewrite. Those are viable, but they would change the interaction model we already use.

2. Persist user account data in a dedicated `User` entity.
   - Rationale: the user-account store should be owned by the application persistence layer, not by the controller or view layer.
   - Alternatives considered: storing credentials only in ASP.NET Identity tables or modeling users as contacts. Those do not match the desired ownership model.

3. Represent roles with a fixed enum for the initial role set, while allowing custom claims to extend authorization later.
   - Rationale: the first phase only needs a small, stable set of roles (`User`, `Admin`), and an enum prevents invalid role values. Custom claims should remain available for future authorization needs without forcing a redesign of the account model.
   - Alternatives considered: free-form text roles or a dedicated `Role` entity. Text is less safe, and a full role entity adds unnecessary complexity for phase 1.

4. Store passwords as hashes.
   - Rationale: the persistent `User` entity should never contain plaintext passwords.
   - Alternatives considered: storing plaintext passwords or delegating password storage to a different table. Plaintext is insecure, and separate storage would blur the persistence contract.

5. Use ASP.NET Core cookie authentication for browser sign-in.
   - Rationale: cookie auth is the natural browser session mechanism for MVC apps and supports standard authorization attributes.
   - Alternatives considered: token-only browser auth. That is less convenient for server-rendered MVC flows.

6. Validate cookies on request and coordinate auth-sensitive user maintenance through events.
   - Rationale: validate-on-request catches password, role, and status changes, while command handlers publish lifecycle events that event handlers use to keep the current session and UI notification flow aligned. Shared client events such as app.signout let the browser show a modal and then redirect when sign-out is required.
   - Alternatives considered: validate only once at sign-in. That approach is less predictable than request-time validation plus immediate server-pushed sign-out.

7. Keep command and query paths separate.
   - Rationale: commands should be secured first, while queries can remain open until the requirements change.
   - Alternatives considered: secure everything immediately. That would add friction before the account/command model is settled.

8. Keep the initial auth scope simple.
   - Rationale: the first phase should focus on login, logout, change password, roles, and account maintenance.
   - Alternatives considered: adding password reset and MFA now. Those add support and recovery complexity that is not required yet.

9. Mirror the Contacts UI flow for user maintenance.
   - Rationale: the users page should follow the same page-shell, JavaScript, and query/command interaction pattern already used by Contacts, so the interaction model stays consistent.
   - Alternatives considered: separate heavy admin screens or a completely different UI approach. Those would be inconsistent with the existing app patterns.

10. Use `app.signout` for forced sign-out notifications, but not for routine logout.
    - Rationale: the browser should react to server-driven auth/session decisions with a dedicated shared event and modal when the current user is affected, while voluntary logout can follow a normal flow without a forced-sign-out modal.
    - Alternatives considered: reuse the same signal for every logout or rely only on redirects. Those approaches make the UI flow less clear and less consistent.


## Risks / Trade-offs

- [Risk] Persistent `User` records can diverge from auth state → Mitigation: use a single auth service/manager, validate-on-request cookie checks, immediate server-pushed sign-out events, and explicit synchronization rules.
- [Risk] Cookie auth can expose session risk if not configured carefully → Mitigation: use `HttpOnly`, `Secure`, and a restrictive `SameSite` policy.
- [Risk] Query access remains public initially → Mitigation: treat it as deliberate and revisit when authorization requirements expand.
- [Risk] Custom account persistence requires more code than Identity → Mitigation: keep the first phase narrow and reuse the existing CQRS/persistence patterns.
- [Risk] A fixed enum can be too rigid if role requirements grow → Mitigation: keep `User` and `Admin` as the initial role set and allow custom claims to extend authorization without redesigning the account model.
- [Risk] UI and API flow definitions can drift if they are specified separately → Mitigation: keep the UI spec aligned to the query/command route contract, SignalR notification contract, shared client auth/session events, and the Contacts-style page-shell plus JavaScript interaction model.

## Migration Plan

1. Add MVC auth controllers and views for login, logout, and change password.
2. Add a persistent `User` entity and user-management route flow.
3. Define the initial `UserRole` enum values and password-hash persistence contract in the spec.
4. Capture the UI layout/navigation and the user-maintenance page-shell and JavaScript interaction pattern in the UI spec.
5. Bridge account authentication to the persistent user store.
6. Add cookie authentication with validate-on-request, and authorization middleware.
7. Add user lifecycle events and event-handler orchestration for auth-sensitive commands.
8. Add shared client auth/session events such as app.signout to the UI bootstrap layer.
9. Secure command middleware with the authenticated principal, initial roles, and extensible claims.
10. Keep query middleware public in this phase.
11. Add query authorization later if requirements change.

## Open Questions

- Should the persistent `User` entity keep only account fields, or also store app-specific profile fields?
- Should command authorization remain role-based for now and rely on custom claims only in a later change?
- Should the user CQRS command names remain `UserCreate` / `UserUpdate` / `UserDisable` / `UserDelete`, or should they be aligned to existing command naming conventions more tightly?
