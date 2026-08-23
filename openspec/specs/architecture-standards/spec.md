# Application Architecture and Development Standards

## Purpose

This document defines the default architecture and implementation conventions for CoConnect. It is meant to be practical, opinionated, and grounded in the way the current application already works.

## 1. Architectural Principles

The application should remain simple and explicit.

Use these rules as the default:

- Keep UI code focused on presentation and interaction.
- Keep controllers thin and transport-oriented.
- Keep business behavior in handlers or supporting domain services.
- Keep persistence concerns behind the domain boundary.
- Prefer command-driven workflows for state changes.
- Prefer event-driven updates when the UI should react in real time.
- Prefer event-handler orchestration when a workflow must also drive UI notifications or auth/session decisions.

## 2. Layering Rules

### Presentation layer

This layer includes:

- Razor views
- page-level JavaScript
- shared client bootstrap files

Responsibilities:

- render the user interface
- collect user input
- submit commands or queries through the app runtime helpers
- react to real-time events from the server

### API layer

This layer includes:

- controllers
- route definitions
- request parsing

Responsibilities:

- receive HTTP requests
- forward commands to the messaging pipeline
- return query results to the client
- avoid embedding business rules directly

### Domain layer

This layer includes:

- commands
- events
- handlers
- supporting domain logic

Responsibilities:

- implement the actual behaviour for a feature
- perform validation and state transitions
- publish follow-up events when needed

### Persistence layer

This layer includes:

- entities
- specifications
- persistence abstractions and data access helpers

Responsibilities:

- store and retrieve data
- encapsulate query logic
- isolate persistence choices from the rest of the application

## 3. Feature Development Pattern

Every feature should follow the same shape:

1. Add the UI in the relevant Razor view.
2. Initialize page-specific behavior using app.ready.
3. Submit commands with app.post.
4. Load read-only data with app.get.
5. Handle real-time updates with app.on.
6. Implement the server-side command/query pipeline.
7. Persist data using the existing persistence conventions.
8. Publish an event if the UI should update in real time.
9. Use event handlers to coordinate follow-up concerns such as SignalR notifications and, for auth-sensitive flows, cookie/session decisions.
10. Surface auth/session outcomes to the browser through a dedicated client event such as app.signout when the UI must react before redirecting.

This pattern should be treated as the default implementation model for new work, including Contacts-style CRUD and maintenance screens.

## 4. Frontend Standards

### Shared runtime

The client-side bootstrap files are the application shell.

- app-init.js manages readiness and deferred execution
- app-start.js contains shared helpers and runtime behavior
- signalr-hub.js manages SignalR subscriptions and connection lifecycle

The shared runtime is also the place for global client events such as app.error, app.info, and auth/session notifications like app.signout.

### Required helper usage

Use these helpers instead of ad hoc implementations:

- app.ready for page initialization
- app.get for GET requests
- app.post for command submission
- app.showToast for user feedback
- app.showConfirm for confirmation flows
- app.on for subscribing to shared runtime and feature events

Auth/session events such as app.signout may show a modal before redirecting or logging the user out.

### Feature script rules

- Keep feature-specific script logic near the view that uses it.
- Do not duplicate shared UI helpers across pages.
- Do not build page behavior directly against raw jQuery or manual fetch code when the shared app helpers already exist.
- Subscribe to auth/session client events in app-start.js or the relevant shared bootstrap layer when the reaction is global.
- Use a dedicated event such as app.signout when the browser must react to a server-auth/session decision before redirecting.


## 5. Backend Standards

### Controllers

Controllers must remain thin.

They should:

- receive incoming requests
- map request payloads to commands or queries
- delegate work to the appropriate handler or query path
- return standard responses

Controllers should not contain business logic.

### Handlers

Handlers are the primary place for feature behaviour.

Every command handler should:

- perform one clear business operation
- use the persistence layer when data changes are required
- publish an event when the outcome should be visible to the UI
- log meaningful failures and notify the client when appropriate

Every event handler should:

- react to one outcome-oriented event
- coordinate any follow-up UI notification flow through SignalR when needed
- coordinate auth/session decisions for sensitive workflows when needed
- avoid performing unrelated persistence work unless the event specifically requires it

### Services

Use services only when the work is too broad or reusable to keep in a handler.

Prefer:

- a focused handler for a single command
- a supporting service for shared logic that spans multiple features

## 6. Command and Event Conventions

### Commands

Use commands for state-changing actions.

Rules:

- use PascalCase
- use action-oriented names such as ContactCreate or FeatureCreate
- represent one user-initiated intent
- be submitted through app.post and routed to the command pipeline

### Events

Use events for outcomes after a command succeeds.

Rules:

- use PascalCase
- use outcome-oriented names such as ContactCreated or FeatureCreated
- stay minimal and focused on what the client needs to know
- be used to trigger UI updates through SignalR when appropriate
- serve as the handoff point for any event-handler orchestration that must react to the command result

### Event payloads

Keep events small.

Prefer sending:

- identifiers
- the minimal fields needed to refresh the client
- no large or redundant payloads

## 7. API Conventions

### Route structure

Use a consistent route pattern:

- /api/query/... for read operations
- /api/command/... for state-changing operations

### Payload shape

- use JSON request bodies for commands
- use camelCase in client payloads
- keep payloads explicit and easy to map to command classes

### Response conventions

- use standard HTTP status codes
- return meaningful errors for invalid or failed operations
- keep the API contract clear and predictable

## 8. Persistence Conventions

### Entities

Entities should represent persisted state clearly and simply.

They should:

- remain focused on state
- avoid UI-specific or transport-specific concerns

### Specifications

Use specifications for read-side query logic.

They should:

- encapsulate query conditions
- keep query logic out of controllers and handlers

### Data mutation rules

For create, update, and delete flows:

- verify the target exists where required
- save changes through the persistence context
- publish an event after successful mutation when the UI should react
- let event handlers coordinate UI notification and auth/session follow-up when required

## 9. Naming Conventions

Use these naming rules consistently:

- Controllers: PascalCase, descriptive, end with Controller
- Commands: PascalCase, action-oriented
- Events: PascalCase, outcome-oriented
- Handlers: PascalCase with Handler suffix
- Entities: PascalCase, domain-focused

## 10. Feature Implementation Checklist

When adding a new feature, follow this checklist:

1. Decide whether the feature is a query, command, or event-driven workflow.
2. Add the UI in the appropriate Razor view.
3. Initialize page behavior with app.ready.
4. Use app.get for reads and app.post for commands.
5. Add the server-side controller endpoint.
6. Implement the command or query handling path.
7. Add or update persistence logic where needed.
8. Publish an event if the UI should update in real time.
9. Add event-handler orchestration when the workflow needs SignalR or auth/session follow-up.
10. Reuse shared client helpers rather than introducing new patterns.
11. Keep the implementation aligned with the existing architectural shape, including Contacts-style maintenance flows.

## 11. Guiding Principle

Prefer simple, explicit implementation over clever abstractions. The app should remain easy to understand and extend. If a new feature can follow the existing command → handler → persistence → event → UI update flow, it should do so.
