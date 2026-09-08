# ADR-0001: Modular monolith with vertical slices

- **Status:** Accepted
- **Date:** 2026-09
- **Deciders:** Architect (sole maintainer)
- **Relates to:** arc42 [§1](../01-introduction-and-goals.md), [§4](../04-solution-strategy.md), [§5](../05-building-block-view.md)

## Context

Andor covers several distinct business capabilities (budget, investing, personal assets, goals,
identity, onboarding, communications). They evolve at different rates and should not bleed into
each other. At the same time:

- There is **one maintainer** and no ops team ([arc42 §2, OC-1](../02-constraints.md)).
- The top quality goal is **modularity / evolvability**, not independent scalability.
- The system must run **locally with one command** and be cheap to operate.

The natural temptation — "proper" microservices — would buy independent deployability and scaling
at the cost of network boundaries, distributed transactions, per-service infra, and a much larger
operational surface for a solo project.

## Decision

We will build Andor as a **modular monolith**: one codebase with **hard module boundaries**,
organised as **vertical slices**, compiled into a **small number of deployable services** grouped
by area (accounts, assets, users, onboarding, communications, configurations) behind a single
reverse proxy.

Boundary enforcement:

- Each slice is the same seven projects ([arc42 §5.2](../05-building-block-view.md)).
- A slice may reference `Foundation.*` and its own `*.Contracts`, **never** another slice's
  `Domain` / `Application` / `Infrastructure`.
- Cross-slice interaction is **integration events** (async, via the Outbox) or a **published
  contract** (sync). No cross-module database joins.
- CI builds/tests/analyses each module **independently**, which surfaces boundary violations as
  broken isolated builds or unexpected project references.

## Options considered

| Option | Pros | Cons |
|---|---|---|
| **Modular monolith (chosen)** | Strong boundaries; refactor across modules with the compiler's help; one-command local run; minimal infra; can be peeled into services later. | Not independently scalable per capability; boundaries rely on discipline + CI, not the network. |
| Microservices from day one | Independent deploy/scale; physical isolation. | Distributed transactions, network failure modes, N pipelines/dashboards/DBs; huge overhead for one person. |
| Single-project layered app | Simplest to start. | Layers don't stop domains coupling; hard to test/analyse a capability alone; erodes fast. |

## Consequences

### Positive

- A capability can be understood, changed, tested and Sonar-gated on its own.
- Domain code has no framework dependencies and is unit-testable without a host.
- The path to extracting a real service later is short: a slice already has its own contracts,
  DB schema and actor system.

### Negative / accepted trade-offs

- Project count is high; every new module pays a fixed scaffolding cost.
- No per-capability horizontal scaling — a hot module can't scale without the rest (see
  [arc42 §11, R-1](../11-risks-and-technical-debt.md)).
- Discipline is load-bearing: an illegal `using` across slices compiles unless caught in review.

### Follow-up

- Add an architecture fitness function (`NetArchTest` / `ArchUnitNET`) per module to fail the
  build on cross-slice `Domain` references ([arc42 §11, D-6](../11-risks-and-technical-debt.md)).
