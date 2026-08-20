## ADDED Requirements

### Requirement: Query route metadata SHALL define discoverable query endpoints
The system SHALL allow query specifications to declare HTTP route metadata so middleware in the web application can discover new query endpoints and replace controller-based query actions for supported routes.

#### Scenario: A new query spec is discovered at startup
- **WHEN** a specification factory method declares query route metadata
- **THEN** the middleware discovery process SHALL register it as an available query endpoint
- **AND** the route SHALL be available without adding a controller action

### Requirement: Query route metadata SHALL define route, verb, and result shape
The system SHALL record the HTTP method, route template, and result cardinality for each discoverable query endpoint.

#### Scenario: Metadata distinguishes list and detail queries
- **WHEN** a query specification declares metadata for a collection route
- **THEN** the metadata SHALL identify the route as a collection query
- **AND** the middleware SHALL treat the result as a list
- **WHEN** a query specification declares metadata for a single-item route
- **THEN** the metadata SHALL identify the route as a single-item query
- **AND** the middleware SHALL treat the result as one item or not found

### Requirement: Query route discovery SHALL bind route parameters to specification inputs
The system SHALL bind route values from the request path to the parameters required by the query specification factory method.

#### Scenario: Contact id is bound into the query spec
- **WHEN** the middleware handles `GET /api/query/contacts/{id}`
- **THEN** it SHALL bind the route value to the specification input for `contactId`
- **AND** it SHALL invoke the matching specification factory method with that value
