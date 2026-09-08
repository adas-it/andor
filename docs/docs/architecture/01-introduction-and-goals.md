# 1. Introduction & Goals

## 1.1 What is Andor

Andor is a **personal-finance and investment management platform**. It gives one person (and,
later, a household) a single place to manage:

- **Budget** — current accounts, financial movements, categories, budgets, invites to share an account.
- **Investing** — investment assets and positions, FIFO cost calculation for tax purposes.
- **Personal Assets** — physical goods (car, house, bicycle) and their running costs.
- **Goals** — personal goals/projects, linked to financial movements.
- **Identity & Authorization** — users, permissions, licenses.
- **Onboarding** — the public, pre-authentication signup flow.
- **Communications** — outbound notifications (e-mail today), driven by configurable rules/templates.

It is built as a **modular monolith**: one codebase, strict module boundaries, a handful of
independently deployable services. The goal is microservice-style isolation of business
capabilities **without** the operational cost of a full microservice fleet.

## 1.2 Requirements overview

### Functional (essential)

| ID | Requirement |
|---|---|
| F-1 | A visitor can sign up from the landing page with name + e-mail, receive a verification code, and complete signup with a password. |
| F-2 | Completing signup provisions the user's identity and their first budget account, each in its own module, from a single event. |
| F-3 | A user can manage current accounts, financial movements and categories, with all writes validated against domain rules. |
| F-4 | A user can share an account with another person through an invite. |
| F-5 | A user can register investment assets and get FIFO-based cost/gain calculations. |
| F-6 | A user can register personal assets and their expenses, and define goals linked to movements. |
| F-7 | Any module can request a user-facing notification without knowing the channel, wording or provider. |
| F-8 | All business APIs are authenticated with JWT; onboarding is the only anonymous surface. |

### Quality goals (top 3)

The three quality attributes that most drive the architecture, in priority order:

| Priority | Quality goal | Concrete meaning | Where it is addressed |
|---|---|---|---|
| 1 | **Modularity / evolvability** | A business capability can be understood, changed, tested and analysed on its own; cross-module coupling is only through explicit contracts and events. | [Solution Strategy](04-solution-strategy.md), [Building Block View](05-building-block-view.md), [ADR-0001](adr/0001-modular-monolith.md) |
| 2 | **Correctness / consistency** | Expected failures are modelled as values, not exceptions; state changes and the events that announce them are persisted atomically; concurrent writes to one aggregate are serialized. | [Cross-cutting Concepts](08-crosscutting-concepts.md), [ADR-0002](adr/0002-actor-model-for-writes.md), [ADR-0003](adr/0003-transactional-outbox.md), [ADR-0004](adr/0004-result-pattern-over-exceptions.md) |
| 3 | **Operability / observability** | The whole system runs locally with one command; every service exposes health checks and emits traces/metrics/logs through a single pipeline. | [Deployment View](07-deployment-view.md), [Cross-cutting Concepts](08-crosscutting-concepts.md) |

Full, testable quality scenarios are in [chapter 10](10-quality-requirements.md).

## 1.3 Stakeholders

| Role | Interest in the architecture |
|---|---|
| **Product owner / end user** (the author) | Features land quickly and safely; the money numbers are right. |
| **Architect** (the author, portfolio context) | The design demonstrates DDD, clean layering, messaging and .NET platform skills, and is documented well enough to reason about. |
| **Contributor / reviewer** | Can locate code by convention, run a module's tests in isolation, understand why patterns exist. |
| **Operator** | Can stand the system up, watch it, and roll images forward per module. |
| **Automated quality gate** (SonarCloud) | Each module is analysed and gated independently. |
