# ADR-0006: Consumer-owned deserialization types for integration events

- **Status:** Accepted
- **Date:** 2026-09
- **Deciders:** Architect (sole maintainer)
- **Relates to:** arc42 [§5.2](../05-building-block-view.md), [§8.4](../08-crosscutting-concepts.md),
  [ADR-0001](0001-modular-monolith.md), [ADR-0003](0003-transactional-outbox.md)

## Context

[ADR-0001](0001-modular-monolith.md) says cross-slice interaction is **integration events** (async,
via the Outbox) or a **published contract** (sync), and that a slice may reference `Foundation.*`
and its own `*.Contracts`, never another slice's `Domain`. It does not say what shape an
integration event itself should take once it reaches a consumer — that's this ADR.

A `DomainEvent` (e.g. `SignupVerifiedDomainEvent`) is raised inside an aggregate and lives in that
module's `*.Domain` project. It is free to evolve with the aggregate's internal invariants — that's
the point of keeping it in Domain rather than Contracts. [ADR-0003](0003-transactional-outbox.md)
relays it to Azure Service Bus as JSON, with the CLR type's short name/full name on `Subject` /
`ApplicationProperties["MessageType"]` (no assembly info — nothing on the wire ties a consumer to
the producer's assembly).

The question this ADR settles: should a consuming module deserialize that JSON into a **shared
DTO published from the producer's `*.Contracts` project** (mirroring the `*Input`/`*Output` pattern
already used at the REST boundary, with a Domain → Contracts mapper on the publish side), or should
each consumer **declare its own local type** for the fields it needs?

This was never designed — it happened organically. `SignupVerifiedConsumer` (Onboarding) already
deserializes the event Users/Onboarding publishes into a private `record SignupVerifiedMessage`
declared in the consumer file itself, and `Andor.Onboarding.Service` has no project reference to
the publisher's `*.Domain` assembly. QS-M4 ([arc42 §10](../10-quality-requirements.md)) already
treats a cross-module `Domain` reference as a violation that should stand out in review. The open
question was whether to formalize the existing pattern or replace it with a shared Contracts DTO.

## Decision

We will keep integration events **consumer-owned**: each consuming module declares its own
private record/class for the fields it needs and deserializes the Service Bus message body
directly into it (tolerant reader). We will **not** add an "integration event" DTO to any module's
`*.Contracts` project, and we will **not** reference another module's `*.Domain` assembly to share
the event's CLR type.

The publisher's `DomainEvent`-derived class stays internal to its own `*.Domain` project. The only
contract that actually crosses the module boundary is the **JSON shape on the wire** plus the
routing string (`OutboxMessage.Type` → `Subject`/`EventName`), not a shared C# type.

To keep that implicit contract from silently breaking:

- A publisher may only **add** fields to a `DomainEvent` that has cross-module consumers, never
  rename or remove a field a consumer relies on.
- A breaking change to a consumed field requires a new event name (e.g. a `V2` suffix or a bumped
  `EventName`), not an in-place change — the same way a REST contract would be versioned.
- When a `DomainEvent` picks up its first cross-module consumer, note in a comment on the class
  which fields are now public surface, so a future edit doesn't rename one by accident.

## Options considered

| Option | Pros | Cons |
|---|---|---|
| **A — Consumer-owned local type (chosen)** | No compile-time coupling between modules; matches what `SignupVerifiedConsumer` already does; the wire JSON is the real contract either way; each consumer takes only the subset of fields it needs. | No compiler-enforced schema — a producer breaking a field only fails at runtime, in the consumer that used it. |
| B — Shared event DTO in `*.Contracts` + Domain→Contracts mapper | Single documented source of truth for the shape; a consumer that references it gets a compile error on a breaking rename. | Reintroduces a cross-module reference for the one thing ADR-0001 explicitly kept decoupled (events); a consumer now rebuilds on any change to that DTO, including fields it doesn't use; muddies `*.Contracts`, whose established purpose is REST `*Input`/`*Output`, not event payloads. |
| C — Publish the `DomainEvent` type itself, consumers reference the producer's `Domain` assembly | Zero duplication short-term. | Direct violation of the module boundary (QS-M4); leaks Domain-internal invariants and behavior across the wire; tightest possible coupling, the opposite of [ADR-0001](0001-modular-monolith.md). |

## Consequences

### Positive

- No compile-time dependency between modules' `Domain`/`Contracts` projects for eventing; stays
  consistent with QS-M4 and [ADR-0001](0001-modular-monolith.md).
- Codifies what the codebase already does (`SignupVerifiedConsumer`) rather than introducing a new
  pattern — no migration required.
- A publisher can rename or restructure the parts of a `DomainEvent` no consumer relies on without
  touching any other module.

### Negative / accepted trade-offs

- No compiler-enforced schema across the wire: a producer renaming a field a consumer depends on
  breaks that consumer at runtime, not at build time. Mitigated by the append-only/versioning rule
  above; should be paired with the idempotent-consumer follow-up already tracked as
  [R-2](../11-risks-and-technical-debt.md).
- Some duplication: two consumers of the same event each declare their own (usually small) shape.
  Accepted — a shared DTO would tend toward a superset that grows to fit every consumer's needs and
  stops being safely append-only.
- Relies on documentation discipline (the "public fields" comment) rather than tooling; nothing
  currently enforces it automatically.

### Follow-up

- Add the "these fields are public, append-only" comment to `SignupVerifiedDomainEvent` and any
  other `DomainEvent` that already has a cross-module consumer.
- If breakage from this becomes recurring, consider a contract test per event (serialize the
  current type, assert the JSON is a superset of a recorded golden shape) as a fitness function.
