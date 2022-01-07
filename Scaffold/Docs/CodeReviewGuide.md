# Code Review Guide

Code reviews are essential for catching expensive architectural and design issues in a code base. They should be conducted on a regular cadence independent from and during feature delivery. When combined with the commitment to address issues early, code reviews can produce an enjoyable and productive experience for those working with a code base.

The code review process is often conducted by the developers of a code base. It is also often valuable to include an experienced outsider in the code review process. When more people are required to review a code base or when there are multiple code bases to review, it becomes necessary to scale the code review process. This guide is intended to help you scale this process.

## How to Use

The purpose of the code review guide is to help bring transparency and consistency to how code is reviewed. Reviewers are expected to use this guide in combination with their experience and context to surface issues. The code review guide is not a check list.

### Living Document

The code review guide is meant to be a living document and is programming language and framework agnostic. Reviewers are encourage to update and fix the code review guide where it is broken. For certain code bases, it may be helpful to have a separate code review guide that is specific to the programming language or framework used in that code base.

### Level Setting

Before commencing a code review, reviewers may want to familiarize themselves with the language used in the code review guide. In particular reviewers should be familiar with concepts in Domain Driven Design and various architectural patterns and their associated terms. Recommended read for these are:

- _Domain-Driven Design: Tackling Complexity in the Heart of Software_ by Eric Evans
- [_DDD, Hexagonal, Onion, Clean, CQRS, … How I put it all together_](https://herbertograca.com/2017/11/16/explicit-architecture-01-ddd-hexagonal-onion-clean-cqrs-how-i-put-it-all-together/) by Herberto Graça.

### Product vs. Productivity

- The lenses that you can view a code base.

### Outputs

Output of the code review process.

### Structure

How this guide is organised.

- Reliability Engineer / Error Budgets
- SRE Post Mortems

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
- How easy is it to deploy from nothing?
- What prereqs. infrastructure are required to deploy?
- Is deployment easily repeatable?
- How many ambient Environment variables are there?
- How easy is it to tear down?
- How many environments are there? Are they ephemeral?

## Requirements

- Actually does what it says on the box
- Is there any technical debt?
- Are there any feature switches and how do you turn them on?
- Is the change/feature easy to remove? Is it decoupled?

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

## Developer Experience

## .NET Stuff

- Correct use of IDisposable
- High Performance Logging
- Correct use of access modifiers

## Possible Structures

### Short vs. Long Review

### Waterfall Model

- Analyse
- Design
- Build
- Test
- Operate

### DevOps Model

- Continuous Integration
- Continuous Deployment
- Continuous Analysis

### Users vs. Developers

- Product
- Productivity (Developer Experience)
