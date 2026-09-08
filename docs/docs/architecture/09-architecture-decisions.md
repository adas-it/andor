# 9. Architecture Decisions

Significant decisions are recorded as **Architecture Decision Records (ADRs)** — short documents
capturing the context, the decision, and its consequences at the time it was made. The full log
and template are under [Architecture Decision Records](adr/index.md).

## 9.1 Decision log

| ADR | Decision | Status | Key consequence |
|---|---|---|---|
| [ADR-0001](adr/0001-modular-monolith.md) | Build a **modular monolith** with vertical slices instead of microservices. | Accepted | Strong module boundaries, low operational cost; not independently scalable per capability. |
| [ADR-0002](adr/0002-actor-model-for-writes.md) | Handle aggregate writes through **Akka.NET actors** (one actor per aggregate instance, Manager routing, load-then-serve). | Accepted | Serialized writes per aggregate without distributed locks; actor lifecycle complexity. |
| [ADR-0003](adr/0003-transactional-outbox.md) | Publish integration events via a **transactional Outbox** relayed to **Azure Service Bus**. | Accepted | Atomic state + event; eventual consistency; consumers must be idempotent. |
| [ADR-0004](adr/0004-result-pattern-over-exceptions.md) | Represent expected failures with a **Result / Notification** pattern, not exceptions. | Accepted | Deterministic HTTP mapping; every layer threads results. |
| [ADR-0005](adr/0005-schema-per-module-persistence.md) | Give each module its **own `DbContext` and schema**; no cross-module joins or FKs. | Accepted | Real data isolation; some duplication; integration only via contracts/events. |

## 9.2 Decisions still implicit (candidates for future ADRs)

These are in force but not yet written up as ADRs:

- **.NET Aspire** as the local orchestration model and `ServiceDefaults` as the cross-cutting
  host composition root.
- **YARP single reverse proxy** as the only ingress, with path-prefix routing per module.
- **One reusable Dockerfile** parameterised by build args for all deployable services.
- **Per-module CI + SonarCloud project** rather than a single pipeline.
- **OpenIddict / JWT Bearer** as the authentication mechanism across all services.
- **MkDocs Material + arc42**, docs built `--strict` and published from `docs/**`.

## 9.3 How to add an ADR

1. Copy [`adr/template.md`](adr/template.md) to `adr/NNNN-short-title.md` (next number).
2. Fill in *Context → Decision → Consequences*; keep it to one page.
3. Add a row to the log in §9.1 and a nav entry in `docs/mkdocs.yml`.
4. Never edit an accepted ADR's decision retroactively — supersede it with a new one and set the
   old status to *Superseded by ADR-NNNN*.
