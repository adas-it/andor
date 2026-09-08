# ADR-0005: One DbContext and schema per module

- **Status:** Accepted
- **Date:** 2026-09
- **Deciders:** Architect (sole maintainer)
- **Relates to:** arc42 [§5](../05-building-block-view.md), [§8.5](../08-crosscutting-concepts.md), [ADR-0001](0001-modular-monolith.md), [ADR-0003](0003-transactional-outbox.md)

## Context

[ADR-0001](0001-modular-monolith.md) makes module boundaries the primary quality goal. A shared
database with cross-module foreign keys and joins would quietly undo that: a schema change in one
module could break queries in another, and "just join it" becomes the path of least resistance.

Persistence is EF Core 10 on SQL Server ([arc42 §2, TC-2](../02-constraints.md)).

## Decision

Each module owns its data:

- One **`DbContext`** per module (`AccountsContext`, `CommunicationContext`, …), each mapped to
  its **own schema** (`Accounts.*`, `Communication.*`, …).
- **No foreign keys or joins across module boundaries.** If module B needs data owned by module A,
  it either calls A's published contract or keeps its own copy updated from A's integration
  events ([ADR-0003](0003-transactional-outbox.md)).
- Each `DbContext` also hosts that module's **Outbox** table and its `IOutboxContextProvider`.
- Migrations are per-`DbContext`, run from the module's Infrastructure project, and applied on
  service start (`app.Apply<Module>MigrationsAsync()`). Seed data ships as deterministic
  `InsertData` migrations (fixed GUIDs + timestamps).
- Physically, schemas may share a SQL Server instance now; nothing in the code assumes they do.

## Options considered

| Option | Pros | Cons |
|---|---|---|
| **Schema/DbContext per module (chosen)** | Real isolation; a module's tables can move to their own DB with no code change; migrations are small and scoped; enables the Outbox per module. | Data duplication across modules; no DB-level referential integrity across boundaries; some read scenarios need an API call or a projection. |
| Shared DbContext, one schema | Easy joins; one migration history. | Boundary erosion; cross-module change ripple; can't split later without surgery. |
| Separate physical database per module now | Maximum isolation. | Operational cost not justified for a solo project yet — and not needed, since the code already doesn't assume co-location. |

## Consequences

### Positive

- Module boundaries are enforced at the data layer, not just in C#.
- Extracting a module into its own service/database later is a config change, not a rewrite.
- Tenant scoping is applied uniformly per `DbContext` via global query filters
  ([QS-S3](../10-quality-requirements.md#security)).

### Negative / accepted trade-offs

- The same real-world entity (e.g. a user) is represented in more than one module's store, kept
  consistent by events — with the usual eventual-consistency lag.
- Cross-module reporting needs composition at the API layer or a dedicated read model (not built —
  [arc42 §11](../11-risks-and-technical-debt.md)).
- On-startup migrations don't suit multi-replica rollouts as-is
  ([arc42 §11, R-3](../11-risks-and-technical-debt.md)).

### Follow-up

- Move migration execution to a release/init step before running more than one replica per
  service.
