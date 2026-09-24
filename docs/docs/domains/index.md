# Domains

This section is the **working reference for each bounded context** in Andor. Where the
[Architecture (arc42)](../architecture/index.md) section takes the system-level view, these pages
stay inside one slice: its **domain model**, REST **contracts** (`*Input` / `*Output`),
**endpoint tables**, business rules, **`DomainErrorCode` catalogue** and the **domain / integration
events** it publishes or consumes.

Every business capability is built as the same seven-project **vertical slice**
(`*.Domain`, `*.Application`, `*.Contracts`, `*.Infrastructure`, `*.Binder`, `*.RestApi`,
`*.Service`) — see [chapter 5, Building Block View](../architecture/05-building-block-view.md) for
the shape and [ADR-0001](../architecture/adr/0001-modular-monolith.md) for why.

## Domain reference

| Domain | Covers | Status |
|---|---|---|
| [Budget](budget.md) | Shared current accounts, financial movements, categories / sub-categories / payment methods, member invites, and the monthly CashFlow projection. | Implemented — `Andor.Accounts.*`, service `accounts-api` |
| [Investing](investing.md) | Investment areas, tickers and buy/sell movements; FIFO cost basis for tax (planned). | Early-stage skeleton — `Andor.Assets.*`, service `assets-service` |
| [Personal Assets](personal-assets.md) | Physical possessions (car, house, bicycle, equipment) and their running costs, feeding a net-worth view. | Design placeholder — no code yet |
| [Goals](goals.md) | Personal financial goals / projects with a target amount and date, progress tracked from linked movements. | Design placeholder — no code yet |
| [Onboarding](onboarding.md) | Public signup flow (name + e-mail → verification code → password), the only anonymous entry point. | Implemented — `Andor.Onboarding.*`, service `onboarding-api` |
| [Communications](communications.md) | Rule / Template model for outbound notifications (e-mail today), invoked via queue or REST. | Implemented — `Andor.Communications.*`, service `communications-api` |

## How these fit together

- **Onboarding** starts a user off: `SignupCodeGenerated` makes **Communications** e-mail the
  code, and on verification it asks the **Users** module to provision the User. Users then
  orchestrates Identity credentials and a personal **Budget** account, and publishes
  `UserCreatedDomainEvent` on `andor-users-events` — which feeds Communications' Recipient
  projection and triggers Onboarding's welcome e-mail.
- **Budget** publishes `Account*` events on `andor-accounts-events`; its own consumers seed a
  new account with default templates and maintain the CashFlow projection.
- **Communications** is called by any other slice through the `request-communication` queue —
  callers reference a pre-configured Rule/Template and pass only dynamic values. Communications
  itself enriches/consent-gates the request and republishes onto `send-communication`, which only
  its own Azure Function consumes.
- **Investing**, **Personal Assets** and **Goals** are independent slices; the placeholders
  describe how each would plug into the same event-driven model.

Cross-slice flows are traced step by step in
[chapter 6, Runtime View](../architecture/06-runtime-view.md).

## Page shape

[Budget](budget.md), [Onboarding](onboarding.md) and [Communications](communications.md) follow
the full template: **Description → Domain model → Endpoints → Contracts → Business rules →
Domain error codes → Domain events → Integration**. [Investing](investing.md) documents the
current skeleton and flags what is still missing; [Personal Assets](personal-assets.md) and
[Goals](goals.md) are design placeholders for slices that are not built yet.

!!! note "Status"
    Andor is a personal / portfolio project. Each page documents its slice **as built**;
    unfinished areas are called out explicitly on the page and in
    [chapter 11, Risks & Technical Debt](../architecture/11-risks-and-technical-debt.md).
