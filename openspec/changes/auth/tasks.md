## 1. Authentication foundation

- [ ] 1.1 Add MVC login and logout actions and views
- [ ] 1.2 Add change-password support for signed-in users
- [ ] 1.3 Configure ASP.NET Core cookie authentication
- [ ] 1.4 Add periodic cookie revalidation support and sign-out handling

## 2. Persistent user storage

- [ ] 2.1 Add a dedicated persistent `User` entity
- [ ] 2.2 Add a `UserRole` enum for supported roles
- [ ] 2.3 Add repository/specification support for user queries
- [ ] 2.4 Add user-account persistence rules for password hashes, roles, and status

## 3. User maintenance and CQRS flow

- [ ] 3.1 Add `UsersController` routes for query and command actions
- [ ] 3.2 Bridge `UsersController` to the persistent user store
- [ ] 3.3 Keep the user maintenance flow aligned with the Contacts CQRS pattern
- [ ] 3.4 Add disable/enable and role update commands as needed
- [ ] 3.5 Add an orchestration handler for `UserUpdate`, `UserDisable`, and `UserDelete`

## 4. UI and navigation

- [ ] 4.1 Define login screen behavior and validation
- [ ] 4.2 Define change-password screen behavior
- [ ] 4.3 Define users maintenance screen behavior
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
- [ ] 6.4 Verify command routes require authentication
- [ ] 6.5 Verify the solution builds successfully
- [ ] 6.6 Verify periodic cookie revalidation refreshes or invalidates auth-sensitive sessions
