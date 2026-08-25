## Why

Contact maintenance currently occupies the Home page, making the landing page responsible for both navigation and an operational CRUD workflow. Moving maintenance into dedicated pages creates a clearer application structure and establishes a navigation model that can grow into a future dashboard without exposing administration features to unauthorized users.

## What Changes

- Move Contact maintenance from Home to a dedicated Contacts page at `/Contacts`.
- Keep the existing Users maintenance page at `/Users`.
- Add a Maintenance navigation group with Contacts and Users entries.
- Show Users in navigation only to Admin users, while retaining server-side authorization for direct access.
- Show Contacts to every authenticated user.
- Flatten the Maintenance group to a direct Contacts entry when Contacts is its only visible child.
- Hide Maintenance entirely when no maintenance entries are visible.
- Remove Contact CRUD from Home and replace it with a small authenticated interim landing state until a dashboard is introduced.

## Capabilities

### New Capabilities

- `maintenance-navigation`: Dedicated Contacts maintenance routing and conditional navigation for Contacts and Users based on authentication and role access.

### Modified Capabilities

<!-- No existing capability specifications currently define this behavior. -->

## Impact

- Affected Razor views: shared navigation, Home, Contacts, and Users.
- Affected MVC routing: a dedicated Contacts controller/page while preserving the Users route.
- Affected client-side page scripts: Contact CRUD behavior moves with the Contacts page.
- Affected authorization surface: Contacts requires authentication; Users remains Admin-only for both navigation and direct page access.
- No new external dependencies or persistence changes are required.