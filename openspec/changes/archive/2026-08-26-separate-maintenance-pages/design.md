## Context

The shared layout currently renders Home, Privacy, and an Admin-only Users link. Contact maintenance, including its page-specific JavaScript, is embedded in `Views/Home/Index.cshtml`. `UsersController` already protects the Users page with an Admin role authorization attribute. The existing Contact query and command routes can remain unchanged while the UI moves to a dedicated page.

## Goals / Non-Goals

**Goals:**

- Give Contacts and Users stable, dedicated maintenance routes.
- Centralize conditional maintenance navigation in the shared layout.
- Preserve the existing Contact CRUD behavior and shared app runtime conventions.
- Keep UI visibility and server-side authorization aligned for Users.
- Leave Home as a minimal authenticated landing page that can later become a dashboard.

**Non-Goals:**

- Redesigning Contact or User CRUD behavior.
- Changing Contact persistence, ownership, or query/command contracts.
- Introducing dashboard widgets or dashboard data.
- Adding a new permission system beyond the existing authentication and Admin role.

## Decisions

### Use a dedicated Contacts controller and view

Add a thin Contacts controller with an Index action and move the current Contact maintenance markup and page script into a Contacts view. This matches the existing Users page structure and keeps Home focused on landing content. The existing `/api/query/contacts` and command endpoints remain the data boundary.

Alternative considered: keep the Contact UI in Home and link to an anchor. Rejected because it leaves the landing page coupled to a maintenance workflow and does not establish a separate maintenance page.

### Build navigation from visible entries

The shared layout will define the visible maintenance entries for the current request: Contacts for authenticated users and Users for Admin users. It will render a dropdown only when both entries are visible, render Contacts directly when it is the sole entry, and render nothing when there are no entries.

Alternative considered: always render a Maintenance dropdown and disable unavailable entries. Rejected because the requirement is to hide inaccessible items and flatten a single-child group.

### Keep authorization independent of menu rendering

Navigation checks are presentation behavior only. Contacts will require authentication, and Users will retain explicit Admin authorization at the controller boundary so hidden links cannot be bypassed with direct URLs.

Alternative considered: rely solely on hidden navigation links. Rejected because URL access must be protected at the server boundary.

### Use a minimal interim Home view

Replace the Contact UI on Home with a small authenticated welcome state and a link to Contacts. This gives the route useful content without introducing dashboard assumptions.

Alternative considered: leave Home blank. Rejected because a minimal landing state provides a clearer post-login destination while remaining easy to replace later.

## Risks / Trade-offs

- [Risk] Moving the script without preserving its page-specific event subscriptions could break real-time Contact updates. -> Mitigation: move the complete Contact markup and script together, then exercise create, update, delete, initial load, and SignalR refresh flows on `/Contacts`.
- [Risk] A navigation condition could diverge from controller authorization. -> Mitigation: retain controller authorization and test both rendered navigation and direct route access for Admin and non-Admin users.
- [Risk] Existing links or bookmarks to Home may no longer show Contact maintenance. -> Mitigation: provide a visible Contacts link from the new Home landing state and navigation; no API contract changes are needed.

## Migration Plan

1. Add the Contacts page and move the existing Contact UI behavior.
2. Remove Contact maintenance from Home and add the interim landing state.
3. Update shared navigation with conditional Maintenance rendering.
4. Verify authenticated and authorization scenarios, then build the application.

Rollback consists of restoring the Contact markup and script to Home and reverting the navigation/controller view changes; Contact API routes and persisted data are unaffected.