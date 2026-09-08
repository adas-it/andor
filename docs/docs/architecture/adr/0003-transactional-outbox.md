# ADR-0003: Transactional Outbox + Azure Service Bus for integration events

- **Status:** Accepted
- **Date:** 2026-09
- **Deciders:** Architect (sole maintainer)
- **Relates to:** arc42 [§3.2](../03-context-and-scope.md), [§6.3](../06-runtime-view.md), [§8.4](../08-crosscutting-concepts.md), [ADR-0001](0001-modular-monolith.md), [ADR-0002](0002-actor-model-for-writes.md)

## Context

With module boundaries in place ([ADR-0001](0001-modular-monolith.md)), modules must react to each
other's changes without sharing a database or calling each other synchronously. The classic
failure mode is a **dual write**: commit the aggregate, then publish a message — and crash in
between, losing the event (or publish, then fail to commit, announcing something that didn't
happen).

Constraints: Azure Service Bus is the available transport ([arc42 §2, TC-6](../02-constraints.md));
each module has its own SQL schema ([ADR-0005](0005-schema-per-module-persistence.md)).

## Decision

We will use the **Transactional Outbox** pattern:

- When an aggregate needs to announce something, an **`OutboxMessage`** row is persisted **in the
  same `SaveChanges`/transaction** as the aggregate state (per-module wiring via
  `IOutboxContextProvider`, e.g. `CommunicationOutboxContextProvider`).
- A shared, module-agnostic **`OutboxDispatcher`** (`BackgroundService`) polls **every** registered
  `IOutboxContextProvider` and relays pending messages through **`IMessageSenderInterface`**
  (Azure Service Bus in production, configured by `ServiceBusIoc.WithAzureServiceBusMessaging`).
- **Consumers** are per-service `BackgroundService`s. On success they `CompleteMessageAsync`; on a
  handled failure they `AbandonMessageAsync`, letting Service Bus retry and eventually
  dead-letter.
- Application code **never** calls Service Bus inline during a request.

## Options considered

| Option | Pros | Cons |
|---|---|---|
| **Transactional Outbox (chosen)** | State + event are atomic; survives crashes; transport-agnostic; at-least-once with retry/DLQ for free. | Eventual consistency; consumers must be idempotent; a polling dispatcher adds latency + a moving part. |
| Direct publish after commit | Simple. | Dual-write: lost or phantom events on failure. |
| Full distributed transaction (2PC across DB + broker) | Strong consistency. | Not supported by Service Bus + SQL cleanly; poor availability; heavy. |
| Change Data Capture / Debezium | No app changes to write path. | Extra infra to run; couples to DB internals; overkill here. |

## Consequences

### Positive

- [QS-C2](../10-quality-requirements.md#correctness-consistency): a crash right after the write
  still delivers the event on restart.
- Producers stay fast — they persist a row and return; downstream work is off the request path
  ([QS-P1](../10-quality-requirements.md#performance-efficiency)).
- Swapping or adding a transport is a `Foundation.Infrastructure` change, not a per-module one.

### Negative / accepted trade-offs

- Modules are **eventually consistent** with each other; UIs/consumers must tolerate lag.
- **At-least-once** delivery ⇒ every consumer needs an idempotency strategy; not all have one yet
  ([arc42 §11, R-2](../11-risks-and-technical-debt.md)).
- Dispatcher poll interval is a latency/throughput knob to tune.

### Follow-up

- Give each consumer an explicit dedupe key / upsert.
- Consider push-based outbox relay (e.g. triggered flush on write) if poll latency matters.
