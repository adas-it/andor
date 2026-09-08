# Andor — Architecture Documentation

Andor is a **modular-monolith** personal-finance and investment platform: budget accounts,
investment assets, personal assets, goals, user identity & authorization, onboarding and outbound
communications, delivered as independently testable **vertical slices** inside a small set of
deployable .NET services.

This site has two parts:

- **[Domains](domains/index.md)** — the working reference for each bounded context: domain model,
  REST **contracts** (`*Input` / `*Output`), **endpoints**, business rules and **domain error
  codes**. This is where day-to-day API and integration detail lives.
- **[Architecture (arc42)](architecture/index.md)** — the system-level view: goals, constraints,
  structure, runtime flows, deployment and cross-cutting concepts, plus a log of
  **Architecture Decision Records (ADRs)**.

The [**Domains**](domains/index.md) page lists every bounded context, its status and how the
slices fit together.

## Where to start (architecture)

| If you want to… | Read |
|---|---|
| Understand what the system is for and its top quality goals | [1. Introduction & Goals](architecture/01-introduction-and-goals.md) |
| See the fixed rules the architecture had to obey | [2. Constraints](architecture/02-constraints.md) |
| See the system boundary and its external partners | [3. Context & Scope](architecture/03-context-and-scope.md) |
| Understand the big moves that shape everything else | [4. Solution Strategy](architecture/04-solution-strategy.md) |
| Navigate the code structure | [5. Building Block View](architecture/05-building-block-view.md) |
| Follow the important flows step by step | [6. Runtime View](architecture/06-runtime-view.md) |
| See how it is packaged and deployed | [7. Deployment View](architecture/07-deployment-view.md) |
| Understand the recurring patterns (Result, actors, outbox, multi-tenancy, auth) | [8. Cross-cutting Concepts](architecture/08-crosscutting-concepts.md) |
| Know *why* a given approach was chosen | [9. Architecture Decisions](architecture/09-architecture-decisions.md) / [ADRs](architecture/adr/index.md) |

## Quality gates

Each module is analysed independently on SonarCloud (organization `adas-it`):
[accounts](https://sonarcloud.io/project/overview?id=adas-it_andor-accounts) ·
[investments](https://sonarcloud.io/project/overview?id=adas-it_andor-investments) ·
[communications](https://sonarcloud.io/project/overview?id=adas-it_andor-communications) ·
[configurations](https://sonarcloud.io/project/overview?id=adas-it_andor-configurations) ·
[onboarding](https://sonarcloud.io/project/overview?id=adas-it_andor-onboarding) ·
[users-identity](https://sonarcloud.io/project/overview?id=adas-it_andor-users-identity).
