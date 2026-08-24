## 1. Authentication foundation

- [x] 1.1 Add MVC login and logout actions and views
- [x] 1.2 Add change-password support for signed-in users
- [x] 1.3 Configure ASP.NET Core cookie authentication
- [x] 1.4 Add validate-on-request cookie safety and sign-out handling

## 2. Persistent user storage

- [x] 2.1 Add a dedicated persistent `User` entity
- [x] 2.2 Add a `UserRole` enum for the initial roles
- [x] 2.3 Add repository/specification support for user queries
- [x] 2.4 Add user-account persistence rules for password hashes, roles, and status

## 3. User maintenance and CQRS flow

- [x] 3.1 Add a user-maintenance page shell with JavaScript-driven query and command calls
- [x] 3.1a Document the in-memory bootstrap `Admin` user
- [x] 3.2 Bridge user maintenance commands to the persistent user store
- [x] 3.3 Keep the user maintenance flow aligned with the Contacts page-shell and JavaScript pattern
- [x] 3.4 Add create/update/disable/delete commands as needed
- [x] 3.5 Publish `UserCreated`, `UserUpdated`, `UserDisabled`, and `UserDeleted` events after successful user commands
- [x] 3.6 Add event-handler orchestration for auth-sensitive user session decisions, SignalR notifications, and client auth/session events such as app.signout

## 4. UI and navigation

- [x] 4.1 Define login screen behavior and validation
- [x] 4.2 Define change-password screen behavior
- [x] 4.3 Define the user-maintenance view behavior, JavaScript interactions, and auth/session client event handling
- [x] 4.4 Define shared layout and navigation visibility rules
- [x] 4.5 Keep Contacts UI behavior unchanged

## 5. Command and query protection

- [x] 5.1 Secure command middleware with authentication
- [x] 5.2 Apply role-based authorization to protected command routes
- [x] 5.3 Keep query middleware public for now

## 6. Validation

- [x] 6.1 Verify login and logout succeed through MVC
- [x] 6.2 Verify password changes update the persistent user record
- [x] 6.3 Verify user query routes read from the persistent store
- [x] 6.4 Verify command routes require authentication and role-based authorization
- [x] 6.5 Verify the solution builds successfully
- [x] 6.6 Verify validate-on-request refreshes or invalidates auth-sensitive sessions
- [x] 6.7 Verify user lifecycle events trigger SignalR notifications, session decisions, and client auth/session events such as app.signout
