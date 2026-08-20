## ADDED Requirements

### Requirement: Query middleware SHALL route contact query requests
The system SHALL intercept supported `GET /api/query/...` requests in middleware and route them directly to query handlers when a matching query route exists.

#### Scenario: Contact collection request is routed by middleware
- **WHEN** a request is received for `GET /api/query/contacts`
- **THEN** the middleware SHALL resolve the request to the query handler for the contact collection route
- **AND** the middleware SHALL handle the request without controller involvement

#### Scenario: Contact detail request is routed by middleware
- **WHEN** a request is received for `GET /api/query/contacts/{id}`
- **THEN** the middleware SHALL resolve the request to the query handler for the contact detail route
- **AND** the middleware SHALL handle the request without controller involvement

### Requirement: Query middleware SHALL execute specifications through the query context factory
The system SHALL use `IQueryContextFactory` to create query contexts and SHALL execute matched specifications through those query contexts.

#### Scenario: Query middleware reads contacts without tracking
- **WHEN** the middleware resolves a contact query route
- **THEN** it SHALL create a query context through `IQueryContextFactory`
- **AND** it SHALL execute the associated specification through that query context
- **AND** the result SHALL be returned as read-only data

### Requirement: Query middleware SHALL support collection and single-item query results
The system SHALL return collection results for list routes and single-item results for item routes.

#### Scenario: Collection route returns many contacts
- **WHEN** the request matches the contact collection query route
- **THEN** the middleware SHALL execute the matching specification as a collection query
- **AND** the response body SHALL contain a list of contacts, including an empty list when no contacts exist

#### Scenario: Detail route returns one contact or not found
- **WHEN** the request matches the contact detail query route
- **THEN** the middleware SHALL execute the matching specification as a single-item query
- **AND** the middleware SHALL return `404 Not Found` when no contact exists
