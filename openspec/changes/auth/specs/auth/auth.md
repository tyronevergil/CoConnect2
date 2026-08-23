## ADDED Requirements

### Requirement: The application SHALL support MVC-based login and logout
The system SHALL allow users to sign in and sign out through MVC controller actions and views.

#### Scenario: Successful login
- **WHEN** a user submits valid credentials through the login form
- **THEN** the system SHALL authenticate the user
- **AND** establish a browser session using cookie authentication

#### Scenario: Logout
- **WHEN** an authenticated user signs out
- **THEN** the system SHALL clear the browser session

### Requirement: The application SHALL support change-password for signed-in users
The system SHALL allow an authenticated user to change their password without leaving the MVC account flow.

#### Scenario: Change password
- **WHEN** an authenticated user submits their current password and a new password
- **THEN** the system SHALL verify the current password
- **AND** update the stored password hash

### Requirement: The application SHALL periodically revalidate authenticated cookies
The system SHALL periodically validate the current cookie principal against persisted user account state so that password, role, and status changes are detected.

#### Scenario: Cookie remains valid within the revalidation window
- **WHEN** an authenticated user submits a request before the revalidation interval expires
- **THEN** the system SHALL allow the request to proceed using the existing cookie principal

#### Scenario: Cookie is revalidated after auth-relevant user changes
- **WHEN** the system detects that the persisted `User` record has changed in a way that affects authentication or authorization
- **THEN** the system SHALL refresh the principal or reject the cookie according to the current account state

### Requirement: The application SHALL persist user account data in a dedicated User entity
The system SHALL store account-maintenance data in a persistent `User` entity owned by the application persistence layer.

#### Scenario: Create persistent user record
- **WHEN** an administrator creates a user account
- **THEN** the system SHALL create a persistent `User` record
- **AND** store the account-maintenance fields in the persistence layer

#### Scenario: Update persistent user record
- **WHEN** an administrator updates a user account
- **THEN** the system SHALL persist the changes in the `User` entity

### Requirement: The application SHALL represent user roles with a fixed enum
The system SHALL store the user role as a fixed enumerated value rather than a free-form text field.

#### Scenario: Valid role is assigned
- **WHEN** a user account is created or updated
- **THEN** the system SHALL assign one of the supported enum values
- **AND** persist the role with the user record

#### Supported role values
- `User`
- `Admin`

### Requirement: The application SHALL store the password as a hash
The system SHALL store the user password as a hash and SHALL not persist plaintext passwords.

#### Scenario: Password is saved securely
- **WHEN** a password is created or changed
- **THEN** the system SHALL store the password hash in the persistent `User` entity

### Requirement: The application SHALL track whether a user account is disabled
The system SHALL persist whether a user account is enabled or disabled.

#### Scenario: Account status changes
- **WHEN** an administrator disables or enables a user account
- **THEN** the system SHALL update the persistent `User` status field

### Requirement: The application SHALL publish user lifecycle events and orchestrate auth/session decisions from those events
The system SHALL publish `UserCreated`, `UserUpdated`, `UserDisabled`, and `UserDeleted` events after the corresponding user command succeeds and SHALL use event handlers to keep the persistent `User` record, the current authentication session, and the UI notification flow in sync.

#### Scenario: Successful user command publishes a lifecycle event
- **WHEN** a `UserCreate`, `UserUpdate`, `UserDisable`, or `UserDelete` command completes successfully
- **THEN** the system SHALL publish the matching `UserCreated`, `UserUpdated`, `UserDisabled`, or `UserDeleted` event

#### Scenario: Current-user lifecycle event updates the session
- **WHEN** an event affects the currently signed-in user
- **THEN** the system SHALL refresh the cookie principal, sign the user out, or leave the session unchanged according to the account state
- **AND** publish a shared client auth/session event such as `app.signout` when the browser must react before redirecting
- **AND** notify subscribed clients through SignalR when a session decision is made

#### Scenario: Other-user lifecycle event does not disrupt the current session
- **WHEN** an event affects a different user's account state
- **THEN** the system SHALL persist the change and notify subscribed clients without forcing a refresh of unrelated sessions

### Requirement: The application SHALL secure command middleware with authentication and roles
The system SHALL require authenticated principals for command routes and SHALL allow authorization checks based on roles.

#### Scenario: Authenticated command request succeeds
- **WHEN** an authenticated user submits a command request
- **THEN** the system SHALL allow the request to proceed

#### Scenario: Unauthenticated command request is blocked
- **WHEN** an unauthenticated request submits a command
- **THEN** the system SHALL reject the request with an authorization failure response

### Requirement: Query middleware SHALL remain public for now
The system SHALL allow supported query endpoints to remain publicly accessible in this release.

#### Scenario: Public query request succeeds
- **WHEN** a client submits a supported query request without authentication
- **THEN** the system SHALL allow the request to proceed

### Requirement: The application SHALL expose user maintenance through a page shell and JavaScript-driven query and command calls
The system SHALL render the user-maintenance experience through a thin MVC shell while JavaScript calls the user query and command endpoints directly.

#### Scenario: User query data loads through the page shell
- **WHEN** the user-maintenance page loads
- **THEN** the browser SHALL call the user query endpoint(s) to retrieve persisted users

#### Scenario: User command submits through JavaScript
- **WHEN** an administrator submits create, update, disable, or delete from the page
- **THEN** JavaScript SHALL post the corresponding command to the command endpoint
- **AND** the browser SHALL react to auth/session notifications delivered through SignalR and shared client events such as `app.signout` when applicable

### Requirement: The application SHALL expose user query routes
The system SHALL provide query routes for reading user records from the persistent store.

#### Scenario: List users
- **WHEN** a client requests the user list
- **THEN** the system SHALL return the persisted user records

#### Scenario: Get user by id
- **WHEN** a client requests a specific user by id
- **THEN** the system SHALL return the matching `User` record

### Requirement: The application SHALL expose user command routes
The system SHALL provide command routes for maintaining user records.

#### Scenario: Create user
- **WHEN** an administrator submits a user-create command
- **THEN** the system SHALL create a persistent `User` record

#### Scenario: Update user
- **WHEN** an administrator submits a user-update command
- **THEN** the system SHALL update the persistent `User` record

#### Scenario: Disable user
- **WHEN** an administrator submits a user-disable command
- **THEN** the system SHALL mark the persistent `User` record as disabled

#### Scenario: Delete user
- **WHEN** an administrator submits a user-delete command
- **THEN** the system SHALL remove the persistent `User` record if deletion is supported

#### Suggested route shape
- `GET /api/query/users`
- `GET /api/query/users/{id}`
- `POST /api/command/UserCreate`
- `POST /api/command/UserUpdate/{id}`
- `POST /api/command/UserDisable/{id}`
- `POST /api/command/UserDelete/{id}` if needed
