# ADR-0004: Result / Notification pattern instead of exceptions

- **Status:** Accepted
- **Date:** 2026-09
- **Deciders:** Architect (sole maintainer)
- **Relates to:** arc42 [§4](../04-solution-strategy.md), [§6.1](../06-runtime-view.md), [§8.2](../08-crosscutting-concepts.md)

## Context

Business operations have many **expected** failure paths: validation failures, "not found",
"already verified", "rule has no matching template". Modelling these as exceptions makes them
invisible in signatures, couples control flow to `try/catch`, is easy to swallow or over-catch,
and makes consistent HTTP status mapping ad-hoc. It also pollutes traces/logs with non-errors.

## Decision

We will represent expected outcomes as **values**, threaded through the layers:

```
DomainResult / Notification   (domain)      → error codes, never throws for business rules
        ↓
ApplicationResult<T>          (application) → Data + accumulated errors/warnings/infos,
                                              implicit conversion from T
        ↓
DefaultResponse<T>            (HTTP)        → { data, errors, traceId }
```

- Domain operations return `DomainResult`; a rule violation is a `Notification` /
  `DomainErrorCode`, not an exception.
- The application layer returns `ApplicationResult<T>`.
- **`BaseController.Result<T>(ApplicationResult<T>)` is the single place** HTTP status is
  decided (`200 / 204 / 400 / 404 / 500`), always emitting `DefaultResponse<T>` with the current
  `Activity.TraceId`.
- Exceptions are reserved for the genuinely unexpected; `GlobalExceptionHandlerMiddleware` turns
  them into `500` with a `TraceId` and logs the detail (never leaks it).

## Options considered

| Option | Pros | Cons |
|---|---|---|
| **Result/Notification (chosen)** | Failures are in the type; deterministic status mapping in one place; clean traces; callers can branch on error codes. | Every layer must thread and translate results; more ceremony than `throw`. |
| Exceptions for business failures | Terse happy path. | Hidden control flow; inconsistent handling; noisy telemetry; mapping scattered. |
| Exceptions + global filter mapping types→status | Central mapping. | Still exception-driven flow; still noisy; catch-scope bugs. |

## Consequences

### Positive

- [QS-C1](../10-quality-requirements.md#correctness-consistency): a rule violation is a `400`
  with an error code and `TraceId`, no stack trace, no partial write.
- One obvious place to change response shaping or status rules.
- Domain layer stays free of `HttpStatusCode` and framework types.

### Negative / accepted trade-offs

- Boilerplate: map → call → wrap → return at each boundary.
- Contributors must resist `throw` for expected failures; enforced by review and the shared base
  types.

### Follow-up

- Keep the `DomainErrorCode` catalogue per module documented on the domain pages (Communications
  and Onboarding already list theirs).
