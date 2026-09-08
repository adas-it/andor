# Architecture Decision Records

An **ADR** records one significant architectural decision: the context that forced a choice, the
choice itself, and the consequences we accepted. ADRs are immutable once *Accepted* — a changed
mind is a **new** ADR that supersedes the old one.

Format: lightweight [MADR](https://adr.github.io/madr/)-style. Template: [`template.md`](template.md).

## Log

| # | Title | Status | Date |
|---|---|---|---|
| [0001](0001-modular-monolith.md) | Modular monolith with vertical slices | Accepted | 2026-09 |
| [0002](0002-actor-model-for-writes.md) | Akka.NET actors for the aggregate write model | Accepted | 2026-09 |
| [0003](0003-transactional-outbox.md) | Transactional Outbox + Azure Service Bus for integration events | Accepted | 2026-09 |
| [0004](0004-result-pattern-over-exceptions.md) | Result / Notification pattern instead of exceptions | Accepted | 2026-09 |
| [0005](0005-schema-per-module-persistence.md) | One DbContext and schema per module | Accepted | 2026-09 |

## Relationship to arc42

The ADRs are the detail behind [chapter 4 (Solution Strategy)](../04-solution-strategy.md) and are
indexed from [chapter 9 (Architecture Decisions)](../09-architecture-decisions.md).
