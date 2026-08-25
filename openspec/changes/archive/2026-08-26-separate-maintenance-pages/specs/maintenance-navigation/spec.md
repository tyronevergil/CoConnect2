## Purpose

Provide a dedicated, access-aware maintenance area so authenticated users can manage Contacts without placing CRUD workflows on the Home page, while administrative User maintenance remains restricted.

## ADDED Requirements

### Requirement: Contacts maintenance page

The system SHALL provide a dedicated Contacts maintenance page at `/Contacts` for every authenticated user. The page SHALL provide the existing Contact create, list, update, and delete workflow currently hosted on Home.

#### Scenario: Authenticated user opens Contacts

- **WHEN** an authenticated user navigates to `/Contacts`
- **THEN** the Contacts maintenance page is displayed with the Contact CRUD workflow

#### Scenario: Anonymous user opens Contacts

- **WHEN** an anonymous user navigates to `/Contacts`
- **THEN** the user is denied access and directed through the application authentication flow

### Requirement: Maintenance navigation visibility

The system SHALL render maintenance navigation entries according to the current user's access. Contacts SHALL be visible to every authenticated user, Users SHALL be visible only to Admin users, and the Maintenance group SHALL be hidden when no maintenance entry is visible.

#### Scenario: Admin sees both maintenance entries

- **WHEN** an authenticated Admin views the application navigation
- **THEN** Maintenance is shown as a group containing Contacts and Users

#### Scenario: Non-Admin sees Contacts only

- **WHEN** an authenticated non-Admin views the application navigation
- **THEN** Contacts is shown as a direct navigation entry and Maintenance is not shown as a group

#### Scenario: Anonymous user sees no maintenance entries

- **WHEN** an anonymous user views the application navigation
- **THEN** neither Contacts nor Users nor Maintenance is shown

### Requirement: User maintenance authorization

The system SHALL restrict the Users maintenance page to Admin users independently of whether a navigation entry is rendered. Non-Admin users SHALL NOT be able to access the Users page by navigating directly to its URL.

#### Scenario: Admin opens Users

- **WHEN** an authenticated Admin navigates to `/Users`
- **THEN** the Users maintenance page is displayed

#### Scenario: Non-Admin opens Users directly

- **WHEN** an authenticated non-Admin navigates to `/Users`
- **THEN** access is denied according to the application's authorization behavior

### Requirement: Home is free of maintenance CRUD

The system SHALL remove Contact maintenance controls and Contact CRUD client behavior from Home. Home SHALL remain available as a small authenticated landing state until a dashboard is introduced.

#### Scenario: Authenticated user opens Home

- **WHEN** an authenticated user navigates to Home
- **THEN** Home displays the interim landing state without Contact create, list, update, or delete controls
