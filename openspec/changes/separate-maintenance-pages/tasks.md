## 1. Contacts Page

- [ ] 1.1 Add an authenticated Contacts controller and Index route at `/Contacts`, then verify anonymous requests follow the authentication flow and authenticated requests render the page
- [ ] 1.2 Move the Contact maintenance markup and page-specific JavaScript from Home to the Contacts view, then verify initial loading and Contact create, update, and delete workflows still operate
- [ ] 1.3 Remove Contact CRUD controls and subscriptions from Home and add the interim authenticated landing state with a Contacts link, then verify Home contains no maintenance controls

## 2. Conditional Navigation

- [ ] 2.1 Update shared navigation to expose Contacts to authenticated users and Users only to Admin users, then verify anonymous users see neither entry
- [ ] 2.2 Render Maintenance as a dropdown for users with both visible entries, flatten it to a direct Contacts link for non-Admins, and hide it when no entries are visible; verify each navigation state
- [ ] 2.3 Preserve or add server-side authorization for the Users page and verify a non-Admin cannot access `/Users` directly while an Admin can

## 3. Verification

- [ ] 3.1 Exercise authenticated Admin and non-Admin navigation and direct-page access scenarios, including Contact real-time CRUD updates, and record the observable results
- [ ] 3.2 Run `dotnet build CoConnect/CoConnect.csproj` and verify the solution builds without errors