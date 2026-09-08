# Investing Domain

<details>
  <summary>Diagrama UML - Controle de Investimentos</summary>

![Investing](https://kroki.io/plantuml/svg/eNpLyk9PwzAQhu_9EpiHs0Uau1fSbGahc8TxD2AgSGWmRh5ffuJ6_2gOSZwSWylg0TBMsP_9DaLV-cqwnmSWX6GnR3gcLNuqcpR1fJKqQpiqQxHRkyPjYnRrBCbTgU5HZbbY_hy7OmnRxsvDEz8o8oqOXEPFtWn3F_8FqG9-_)

</details>

## Description

The Investing domain (project family `Andor.Assets.*`, folder `Src/Assets`) is meant to track
**investment positions** — an investor's holdings grouped into *areas*, the *tickers* held in
each area, and the buy/sell *movements* that build a position, with FIFO cost calculation for
tax reporting.

!!! warning "Early-stage module"
    Only the skeleton is in place today: the `Area` aggregate, a bare `Ticker` aggregate and a
    `Movement` entity, one write actor and a single "create area" endpoint. There is **no
    validation on `Area`, no ticker/movement REST surface, no domain events, no error-code
    catalogue and no FIFO logic yet.** The tables below describe what exists in code and flag
    what is still `TODO`.

| Fact | Value |
|---|---|
| Deployable service | `assets-service` (Aspire) |
| Public route prefix (reverse proxy) | `/assets/*` → forwarded with `/assets` stripped |
| Controller base route | `v{version:apiVersion}/Assets` |
| Actor system | `AndorAssetsSystem` |
| DB schema | `investments`, context `InvestmentContext` |
| Domain namespace | `Andor.Assets.Domain.Investments` |

## Domain model

### `Area` — aggregate root (`AreaId`)

A named bucket that groups tickers for one or more members (e.g. *"Retirement"*,
*"Brokerage"*). Implements `ISoftDeletableEntity`.

| Field | Notes |
|---|---|
| `Id` (`AreaId`) | `uniqueidentifier`. |
| `Name` | `Name` value object. |
| `Members` | `Guid[]` — user ids; the creator is added on `NewAsync`. |
| `Tickers` | `Ticker[]` — holdings in this area. |
| `IsDeleted` | Always `false` today (no soft-delete path implemented). |

`Area.NewAsync` currently performs **no validation** and always returns
`DomainResult.Success()`.

### `Ticker` — aggregate root (`TickerId`)

One traded instrument held within an area.

| Field | Notes |
|---|---|
| `Id` (`TickerId`) | `uniqueidentifier`. |
| `Code` | Instrument symbol (e.g. `PETR4`). Required — `ITickerValidator` only checks `Code != null`. |
| `Quotas` | `decimal` — total quantity held. |
| `IsDeleted` | Soft-delete flag. |
| `Movements` | `Movement[]` — the trades that make up the position. |

`Ticker.NewAsync` runs `TickerValidator.ValidateCreationAsync` and would raise a creation
event, but the `RaiseDomainEvent(...)` call is commented out.

### `Movement` — entity under `Ticker` (`MovementId`)

A single buy or sell.

| Field | Notes |
|---|---|
| `Date` | Trade date. |
| `Price` | Unit price. |
| `Quotas` | Quantity traded. |
| `Value` | Total consideration. |

`Movement.New` calls `Validate()` (base only — no field rules yet). There is no explicit
buy/sell direction field and no FIFO matching.

## Write model (actor flow)

`IAreaCommandsService` → `AreaManagerActor` → one `AreaActor` per `AreaId`
(`Manager → Actor → Stash` pattern, same as every other module).

| Command | Status | Effect |
|---|---|---|
| `CreateAreaCommand(AreaId, Name, ApplicationUser, CancellationToken)` | wired | `Area.NewAsync`, returns `AreaOutput`. |
| `AddTickerCommand(AreaId, Name, ApplicationUser, CancellationToken)` | **defined but not handled** — no actor receive, no endpoint. |

`HandleAreaResult` maps domain errors to `ErrorModel`s, but its mapping dictionary is empty.

## Endpoints

Under `v1/Assets`. JWT required (`[Authorize]` on the controller).

| Method | Route | Body | Description |
|---|---|---|---|
| `POST` | `/v1/Assets` | `AreaInput` | Create an investment area for the current user. Returns `AreaOutput`. Handler method is named `GetConfigurationAsync` in code — a copy/paste leftover; the route and behaviour are "create area". |

## Contracts

```csharp
record AreaInput(string Name);
record AreaOutput(string Id, string Name);
```

## Business rules

- An area is created with its creator as the sole member.
- A ticker requires a non-null `Code`.
- **Everything else is still to be defined** — position sizing, buy/sell semantics, average
  price, realised/unrealised P&L, and FIFO cost basis for tax.

## Domain error codes

None defined yet. `HandleAreaResult` has no mappings, so today only generic
validation/`500` envelopes are returned.

## Domain events

None. `Ticker.NewAsync` has a commented-out `RaiseDomainEvent` placeholder; the Assets binder
wires no outbox provider.

## Persistence

- Schema `investments`, tables `Area` and `Ticker` (context `InvestmentContext`).
- Migrations under `Src/Assets/Andor.Assets.Infrastructure` (context `InvestmentContext`).
- No transactional outbox registered for this module yet.

## Next steps

- Define `Ticker` / `Movement` REST contracts and endpoints; handle `AddTickerCommand` in
  `AreaActor`.
- Add real validation to `Area` and `Movement`; introduce a buy/sell direction.
- Implement FIFO cost-basis calculation and expose a tax report.
- Add a domain-error catalogue and wire domain events + outbox (the Budget module is the
  reference implementation).
- Rename `AssetsController.GetConfigurationAsync` to reflect "create area".
