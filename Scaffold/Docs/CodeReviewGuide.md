# Code Review Guide

## The Basics

- Does it build?
- Does it run?
- Do the tests pass?
- Is the code formatted?
- Is the code readable?

## Deterministic Builds

- IaC
- Secrets
- Configuration

## Requirements

- Actually does what it says on the box

## Error Handling

- Custom exceptions inherit the correct base class

## Architecture and Patterns

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

## Adapters

- No business logic in Driving Adapters
- Driven Adapters implement an interface defined in the application layer
- HttpClients are registered correctly in DI

## Observability

- Command/Query Handlers are emitting events
- New trace activity is created in Command/Query Handlers
- Not over logging

## Security and Privacy

- No PII is logged

## Testing

- Test Structure
- Test Data Management
- Test Isolation
- Test Fixtures
- Code Coverage
- Mutation Coverage
- Clear Arrange, Act and Assert
