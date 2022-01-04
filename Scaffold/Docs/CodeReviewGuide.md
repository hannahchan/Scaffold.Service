# Code Review Guide

- Code Reviews should happen on a regular cadence independent from feature delivery
- They can also happen during feature delivery, for example in Pull Requests
- The Code Review Guide is intended to bring transparency and consistency to how code in a project is reviewed.
- The Code Review Guide is a living document and is intended to be updated by the development team overtime to optimise their workflow
- It is also a good place to share knowledge and possible solutions
- Forms part of the developers engineering playbook.

## The Basics

- Does it build?
- Does it run?
- Do the tests pass?
- Is the code formatted?
- Is the code readable?
- Are there warnings and errors?
- Consistent use of language in bounded-context
- Packages are updated

## Deterministic Builds

- IaC
- Secrets
- Configuration

## Requirements

- Actually does what it says on the box
- Is there any technical debt?

## Error Handling

- Custom exceptions inherit the correct base class

## Architecture and Patterns

- Is the Architecture of the application clear and documented
- How are cross cutting concerns addressed by the application architecture
- Domain Driven Design
- Repository Pattern
- No leaky abstractions
- Command, Queries and Events are immutable
- Services are DDD Services
- Interfaces for Adapters are in the correct layer
- Direction of Dependencies
- Correct use of access modifiers
- No excessive use of DI
- Watch out for God Classes
- SOLID
- Composition over inheritance where appropriate

## Adapters

- No business logic in Driving Adapters
- Driven Adapters implement an interface defined in the application layer
- HttpClients are registered correctly in DI
- No excessive network calls
- Are driving adapters part of the application or an external service?
- Are they deployable with the application?

## Observability

- Command/Query Handlers are emitting events
- New trace activity is created in Command/Query Handlers
- Not over logging
- Use High Performance Logging

## Security and Privacy

- No PII is logged
- Careful use of deserialization
- Common OWASP issues

## Testing

- Test Structure
- Test Data Management
- Test Isolation
- Test Fixtures
- Code Coverage
- Mutation Coverage
- Clear Arrange, Act and Assert

## Documentation

- Is documentation required?
- Is documentation updated?
- Comments in code required?

## Database Migrations

- Is a column being renamed? This might drop data?
- Has the migration script been tested?
- Is there any database provider specific code?
- Is there any manual SQL in the migration script?
- Are there backup and restore scripts