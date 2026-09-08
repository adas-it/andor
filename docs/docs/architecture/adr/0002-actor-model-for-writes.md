# ADR-0002: Akka.NET actors for the aggregate write model

- **Status:** Accepted
- **Date:** 2026-09
- **Deciders:** Architect (sole maintainer)
- **Relates to:** arc42 [§5.3](../05-building-block-view.md), [§6.2](../06-runtime-view.md), [§8.3](../08-crosscutting-concepts.md), [ADR-0003](0003-transactional-outbox.md)

## Context

Aggregate writes must be **correct under concurrency**: two commands for the same aggregate must
not interleave and corrupt invariants or lose updates. Options for that are optimistic
concurrency + ret(retry loops), pessimistic DB locks, or a single in-memory owner per aggregate.

We also want the write path to be explicit and uniform across modules, and to have a natural place
to raise domain events alongside the state change.

## Decision

We will route every aggregate command through **Akka.NET actors**:

- A **`*ManagerActor`** per aggregate type receives commands implementing `ICommands<TId>`,
  derives a child actor name from the aggregate id, and forwards — creating the child on first
  use.
- A **`*Actor`** owns exactly one aggregate instance's lifecycle as a `Become` /
  `IWithUnboundedStash` state machine:
  - **`Loading`** — stash incoming mutation commands, self-send a preload message, load the
    aggregate from its `ICommands*Repository` in a **fresh DI scope**, then `Become(Ready)` and
    unstash. Creation commands (`CreateXCommand`) are handled here directly.
  - **`Ready`** — apply mutations, persist the aggregate **and** its `OutboxMessage` in one
    transaction.
- `*CommandsService` classes are the façade controllers call; internally they resolve the
  module's `ActorSystem` and `Ask` the manager.
- Each module starts its own `ActorSystem` (`builder.UseAkkaModules("<SystemName>")`,
  `IAkkaModule`).

## Options considered

| Option | Pros | Cons |
|---|---|---|
| **Actor per aggregate (chosen)** | Serialized writes with **no** locks or retry loops; single owner of aggregate state; clean spot to emit events; back-pressure via mailbox; module-local `ActorSystem` keeps it contained. | Actor lifecycle & `Ask` semantics to learn; state is in-memory (needs reload on restart); not cluster-safe without extra work. |
| EF optimistic concurrency + retry | No new runtime; standard. | Retry storms under contention; every handler must implement retry; races still surface as exceptions. |
| Pessimistic DB locks | Simple mental model. | Lock contention, deadlocks, DB-coupled; poor fit for long-ish handlers. |
| MediatR handlers, no concurrency control | Minimal. | Punts the actual problem. |

## Consequences

### Positive

- [QS-C3](../10-quality-requirements.md#correctness-consistency): concurrent commands for one
  aggregate are processed sequentially by its actor.
- The write path is identical in every module: `Service → Manager → Actor → Repository`.
- Fresh DI scope per load keeps `DbContext` lifetimes short ([QS-P2](../10-quality-requirements.md#performance-efficiency)).

### Negative / accepted trade-offs

- Actors are **single-node, in-memory**. No `Akka.Persistence`/`Cluster.Sharding` yet, so a
  write-heavy module can't be scaled to multiple replicas without two actors racing one aggregate
  id ([arc42 §11, R-1](../11-risks-and-technical-debt.md)).
- More moving parts than a plain handler; contributors must understand stash/become.

### Follow-up

- If a module needs multi-replica scaling, introduce `Akka.Cluster.Sharding` or persistent actors
  behind the same `*CommandsService` façade — callers shouldn't change.
