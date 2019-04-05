# Inventory

Inventory is a simple manager for inventory, like electronic parts, cd library, books or any other stuff
you could have plenty of and just lost control over.

## DDD

DDD is a library created to make building DDD and Event Sourcing easier.
Mainclass, AggregateRoot, implements basic building blocks most of the
domain entities use. Mostly it is event facility.

## DDD.EventStore

This is an attempt to integrate DDD library with an
[Event Store](https://eventstore.org/). For testing
this implementation, a [docker based Event Store](https://hub.docker.com/r/eventstore/eventstore/)
is used.

Once you run it like:
````
docker run --name eventstore-node -it -p 2113:2113 -p 1113:1113 eventstore/eventstore
````

Your event store UI (board) is acessible by this url: [http://localhost:2113/](http://localhost:2113/)


## Incentory.Core

This project is the core of an application. It defines entire domain
and every possible use case.

## Inventory.UI

Finally, this is the UI for Inventory application. It is build with React 
and is actually the playground for learning building UIs with that
library/approach.
