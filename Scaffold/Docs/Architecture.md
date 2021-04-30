# Architecture

The architecture of Scaffold has been heavily inspired by the ideas presented in _DDD, Hexagonal, Onion, Clean, CQRS, … How I put it all together_ by Herberto Graça. You can read his article by clicking [here](https://herbertograca.com/2017/11/16/explicit-architecture-01-ddd-hexagonal-onion-clean-cqrs-how-i-put-it-all-together/).

![Image from Herberto Graça's article](https://herbertograca.files.wordpress.com/2018/11/080-explicit-architecture-svg.png)

## Primary / Driving Adapters

The Primary / Driving Adapters for Scaffold currently live in the [Sources](../Sources) directory along with the Application and Domain Layer.

- [Scaffold.WebApi](../Sources/Scaffold.WebApi)

## Secondary / Driven Adapters

The Secondary / Driven Adapters for Scaffold are currently located in the [Adapters](../Sources/Adapters) directory. These adapters implement [interfaces defined in the Application Layer](../Sources/Scaffold.Application/Interfaces) and their concrete versions are intended to be provided via dependency injection.

- [Scaffold.HttpClients](../Sources/Adapters/Scaffold.HttpClients)
- [Scaffold.Repositories](../Sources/Adapters/Scaffold.Repositories)
