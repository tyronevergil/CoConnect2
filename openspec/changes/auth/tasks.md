## 1. Authentication foundation

- [ ] 1.1 Add MVC login and logout actions and views
- [ ] 1.2 Add change-password support for signed-in users
- [ ] 1.3 Configure ASP.NET Core cookie authentication
- [ ] 1.4 Add validate-on-request cookie safety and sign-out handling

## 2. Persistent user storage

- [ ] 2.1 Add a dedicated persistent `User` entity
- [ ] 2.2 Add a `UserRole` enum for the initial roles
- [ ] 2.3 Add repository/specification support for user queries
- [ ] 2.4 Add user-account persistence rules for password hashes, roles, and status

## 3. User maintenance and CQRS flow

- [ ] 3.1 Add a user-maintenance page shell with JavaScript-driven query and command calls
- [ ] 3.1a Document the in-memory bootstrap `Admin` user
- [ ] 3.2 Bridge user maintenance commands to the persistent user store
- [ ] 3.3 Keep the user maintenance flow aligned with the Contacts page-shell and JavaScript pattern
- [ ] 3.4 Add create/update/disable/delete commands as needed
- [ ] 3.5 Publish `UserCreated`, `UserUpdated`, `UserDisabled`, and `UserDeleted` events after successful user commands
- [ ] 3.6 Add event-handler orchestration for auth-sensitive user session decisions, SignalR notifications, and client auth/session events such as app.signout

## 4. UI and navigation

- [ ] 4.1 Define login screen behavior and validation
- [ ] 4.2 Define change-password screen behavior
- [ ] 4.3 Define the user-maintenance view behavior, JavaScript interactions, and auth/session client event handling
- [ ] 4.4 Define shared layout and navigation visibility rules
- [ ] 4.5 Keep Contacts UI behavior unchanged

## 5. Command and query protection

- [ ] 5.1 Secure command middleware with authentication
- [ ] 5.2 Apply role-based authorization to protected command routes
- [ ] 5.3 Keep query middleware public for now

## 6. Validation

- [ ] 6.1 Verify login and logout succeed through MVC
- [ ] 6.2 Verify password changes update the persistent user record
- [ ] 6.3 Verify user query routes read from the persistent store
- [ ] 6.4 Verify command routes require authentication and role-based authorization
- [ ] 6.5 Verify the solution builds successfully
- [ ] 6.6 Verify validate-on-request refreshes or invalidates auth-sensitive sessions
- [ ] 6.7 Verify user lifecycle events trigger SignalR notifications, session decisions, and client auth/session events such as app.signout
