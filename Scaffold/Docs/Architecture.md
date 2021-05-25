# Architecture

The software architecture of Scaffold has been heavily inspired by the **Explicit Architecture** presented in in _DDD, Hexagonal, Onion, Clean, CQRS, … How I put it all together_ by Herberto Graça. You can read his article by clicking [here](https://herbertograca.com/2017/11/16/explicit-architecture-01-ddd-hexagonal-onion-clean-cqrs-how-i-put-it-all-together/).

![Image from Herberto Graça's article](https://herbertograca.files.wordpress.com/2018/11/080-explicit-architecture-svg.png)

A software architecture provides a common language to help communicate the design and structure of a software system between different software developers.

## Primary / Driving Adapters

The Primary / Driving Adapters for Scaffold currently live in the [Sources](../Sources) directory along with the Application and Domain Layer. Each individual Primary / Driving Adapter is its own [host](https://docs.microsoft.com/aspnet/core/fundamentals/host/generic-host) and communicates with the Application Layer via the Command / Query Bus.

- [Scaffold.WebApi](../Sources/Scaffold.WebApi)

## Secondary / Driven Adapters

The Secondary / Driven Adapters for Scaffold are currently located in the [Adapters](../Sources/Adapters) directory. These adapters implement [interfaces defined in the Application Layer](../Sources/Scaffold.Application/Interfaces) and their concrete versions are intended to be provided via dependency injection.

- [Scaffold.HttpClients](../Sources/Adapters/Scaffold.HttpClients)
- [Scaffold.Repositories](../Sources/Adapters/Scaffold.Repositories)

## Command / Query Bus

The Command / Query Bus in Scaffold uses the mediator pattern which has been implemented with the help of the [MediatR](https://github.com/jbogard/MediatR) library. In this pattern, a Primary / Driving Adapter sends request objects to Command / Query Handlers in the Application Layer and receives responses via a mediator. The Primary / Driving Adapters and Command / Query Handlers do not know about each other.
