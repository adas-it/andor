# 4. Solution Strategy

The handful of decisions below shape everything else. Each one maps a
[quality goal](01-introduction-and-goals.md#12-requirements-overview) or
[constraint](02-constraints.md) to an approach, and links to the ADR with the full rationale.

## 4.1 Decisions at a glance

| # | Decision | Driven by | Trade-off accepted | ADR |
|---|---|---|---|---|
| S-1 | **Modular monolith with vertical slices.** One codebase, hard module boundaries, a few deployable services grouped by area. | Modularity (QG-1), single maintainer (OC-1) | Not independently scalable per capability; discipline (not the network) enforces boundaries. | [ADR-0001](adr/0001-modular-monolith.md) |
| S-2 | **Fixed seven-project shape per slice** (`Domain`, `Application`, `Contracts`, `Infrastructure`, `Binder`, `RestApi`, `Service`). | Modularity (QG-1), per-module CI (OC-2) | Project sprawl; new modules pay a fixed scaffolding cost. | [§5.2](05-building-block-view.md) |
| S-3 | **Domain-Driven Design in a framework-free domain layer** — aggregates, value objects, domain events, repository interfaces; no EF/ASP.NET types. | Correctness (QG-2), evolvability | More mapping code between layers. | [ADR-0004](adr/0004-result-pattern-over-exceptions.md) |
| S-4 | **Result pattern instead of exceptions** for expected failures: `DomainResult` → `ApplicationResult<T>` → `DefaultResponse<T>`. | Correctness (QG-2) | Every layer must thread and translate results. | [ADR-0004](adr/0004-result-pattern-over-exceptions.md) |
| S-5 | **Actor-based write model (Akka.NET).** One actor per aggregate instance, `Loading → Ready` with stash; a `ManagerActor` routes by id. | Correctness (QG-2) — serialize concurrent writes, single owner of aggregate state | Actor lifecycle/'`Ask`' complexity; state lives in memory until loaded. | [ADR-0002](adr/0002-actor-model-for-writes.md) |
| S-6 | **Transactional Outbox + Service Bus** for integration events; a shared `OutboxDispatcher` polls every module's provider. | Correctness (QG-2) — atomic state + event; at-least-once delivery | Eventual consistency between modules; consumers must be idempotent. | [ADR-0003](adr/0003-transactional-outbox.md) |
| S-7 | **Schema/DbContext per module.** No cross-module joins; modules integrate only via contracts + events. | Modularity (QG-1) | Data duplication across modules; no FK across boundaries. | [ADR-0005](adr/0005-schema-per-module-persistence.md) |
| S-8 | **Aspire for orchestration + `ServiceDefaults` for cross-cutting host wiring** (OpenTelemetry, health checks, service discovery, HTTP resilience). | Operability (QG-3), OC-5 | Ties local dev to the Aspire model. |  |
| S-9 | **Single reusable Docker image**, service chosen by build arg; one YARP proxy in front. | Operability (QG-3), TC-8 | All services share a base image lifecycle. | [§7](07-deployment-view.md) |

## 4.2 How the quality goals are met

### QG-1 Modularity / evolvability

- Slices depend on `Foundation.*` and their own `Contracts`, never on another slice's `Domain`/`Infrastructure`.
- The domain layer has **no** framework references, so business rules are unit-testable without a host.
- CI builds and Sonar-analyses each module on its own — a boundary violation shows up as an unexpected project reference or a failing isolated build.
- Cross-module needs are met by **events** (async, Outbox) or **published contracts** (sync), both explicit and versioned.

### QG-2 Correctness / consistency

- **Within an aggregate:** exactly one actor instance handles its commands, so races are impossible without distributed locks; the aggregate raises `DomainEvent`s into its `Events` collection.
- **Across the DB write and its announcement:** the `OutboxMessage` is saved in the same `SaveChanges` as the aggregate — either both persist or neither does.
- **Failure paths:** modelled as `Notification`/`DomainErrorCode` values and propagated up, so a controller maps them to `400/404` deterministically via `BaseController.Result`, and callers can branch on them.

### QG-3 Operability / observability

- `dotnet run --project Andor.AppHost` starts every service + dependencies with one command.
- `AddServiceDefaults()` gives every service the same OpenTelemetry pipeline, `/health` + `/alive` endpoints, service discovery and standard HTTP resilience.
- Traces carry a `TraceId` that also appears in every HTTP response envelope, so a user-reported error can be found in the traces directly.

## 4.3 Patterns used (and where they are documented)

| Pattern | Where |
|---|---|
| Vertical Slice Architecture | [§5](05-building-block-view.md) |
| DDD tactical patterns (Aggregate, VO, Domain Event, Repository) | [§8.1](08-crosscutting-concepts.md) |
| Result / Notification object | [§8.2](08-crosscutting-concepts.md), [ADR-0004](adr/0004-result-pattern-over-exceptions.md) |
| Actor model / one-actor-per-entity | [§8.3](08-crosscutting-concepts.md), [ADR-0002](adr/0002-actor-model-for-writes.md) |
| Transactional Outbox + Consumer | [§8.4](08-crosscutting-concepts.md), [ADR-0003](adr/0003-transactional-outbox.md) |
| API Gateway / reverse proxy | [§7](07-deployment-view.md) |
| Multi-tenant data access (global query filters) | [§8.5](08-crosscutting-concepts.md) |
| Sidecar-free service defaults (composition root) | [§8.7](08-crosscutting-concepts.md) |
