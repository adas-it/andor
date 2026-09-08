# Goals Domain

<details>
  <summary>Diagrama UML - Metas e Projetos</summary>

![Goals](https://kroki.io/plantuml/svg/eNpLyk9OwzAQhu_9EpiHs0Uau1fSbGahc8TxD2AgSGWmRh5ffuJ6_2gOSZwSWylg0TBMsP_9DaLV-cqwnmSWX6GnR3gcLNuqcpR1fJKqQpiQxHRkyPjYnRrBCbTgU5HZbbY_hy7OmnRxsvDEz8o8oqOXEPFtWn3F_8FqG9-_)

</details>

## Description

The Goals domain is meant to let a user define **personal financial goals / projects**
("emergency fund", "trip to Japan", "new laptop"), give each a target amount and date, and
track progress by linking [Budget](budget.md) financial movements (or dedicated contributions)
to the goal.

!!! danger "Not implemented"
    There is **no code for this domain** — no project under `Src/`, no deployable service, no
    entry in the Aspire `AppHost` or the reverse-proxy routes, no database schema, no tests.
    This page is a design placeholder describing the intended scope so the module can be built
    to the same vertical-slice shape as the others.

## Intended scope

| In scope | Out of scope |
|---|---|
| Create a goal: `Name`, `TargetAmount`, optional `TargetDate`, `OwnerUserId` | Holding the money — funds live in [Budget](budget.md) accounts / [Investing](investing.md) positions |
| Attach contributions to a goal (manual amount, or a reference to a `FinancialMovement`) | Automatic transfers between accounts |
| Compute progress: contributed vs target, on-track / behind, projected completion date | Investment return projections |
| Mark a goal `Achieved`, `Abandoned` or `Archived` | Multi-user shared goals (possible later slice) |

## Proposed model (draft)

- **`Goal`** — aggregate root: `Name`, `Description`, `TargetAmount`, `TargetDate?`,
  `OwnerUserId`, `Status` (`Active` / `Achieved` / `Abandoned` / `Archived`), `CreatedAt`.
- **`GoalContribution`** — entity under `Goal`: `Date`, `Amount`, `Source`
  (`Manual` / `LinkedMovement`), optional `FinancialMovementId`.
- Computed: `ContributedAmount`, `RemainingAmount`, `ProgressPercent`,
  `IsOnTrack` (given `TargetDate`).

## Proposed integration

- Emit `GoalCreated` / `GoalContributionAdded` / `GoalAchieved` / `GoalStatusChanged` on a
  `andor-goals-events` topic.
- Optionally **consume** `andor-accounts-events` (`AccountFinancialMovementAddedDomainEvent`)
  so a movement tagged with a goal id is auto-linked as a contribution, keeping the coupling
  to Budget one-directional (Goals depends on Budget's public events, not the reverse).

## Building the module

Follow the seven-project vertical slice used by Budget and Communications:

```
Src/Goals/
  Andor.Goals.Domain
  Andor.Goals.Application       (Manager/Actor/Stash per aggregate)
  Andor.Goals.Contracts        (*Input / *Output)
  Andor.Goals.Infrastructure   (EF Core, schema "Goals", outbox)
  Andor.Goals.Binder           (Ioc + AkkaModule + UseGoals extension)
  Andor.Goals.RestApi          (controllers + UseApi())
  Andor.Goals.Service          (Program.cs)
Tests/Goals/Andor.Goals.Domain.Tests
```

Then register `goals-service` in
`Src/Administrations/Aspire/Andor.AppHost/AppHost.cs` and add a `/goals/*` route to
`Src/Administrations/ReverseProxy/Andor.Admin.ReverseProxy.Yarp/appsettings.json`.

## Next steps

- Confirm the scope and the model above with product.
- Decide how contributions relate to real money (pure tracking vs. reserved balance in an account).
- Scaffold the slice and a first `POST /v1/goal` endpoint plus a progress query.
