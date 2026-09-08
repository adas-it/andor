# Personal Assets Domain

<details>
  <summary>Diagrama UML - Controle de Ativos Pessoais</summary>

![Personal Assets](https://kroki.io/plantuml/svg/eNpLyk9OwzAQhu_9EpiHs0Uau1fSbGahc8TxD2AgSGWmRh5ffuJ6_2gOSZwSWylg0TBMsP_9DaLV-cqwnmSWX6GnR3gcLNuqcpR1fJKqQpiqQxHRkyPjYnRrBCbTgU5HZbbY_hy7OmnRxsvDEz8o8oqOXEPFtWn3F_8FqG9-_)

</details>

## Description

The Personal Assets domain is meant to track **physical possessions** — a car, a house, a
bicycle, electronics — their acquisition value, and the recurring costs of owning them
(insurance, maintenance, taxes, fuel), so that net worth and monthly outgoings reflect more
than just bank balances.

!!! danger "Not implemented"
    There is **no code for this domain** — no project under `Src/`, no deployable service, no
    entry in the Aspire `AppHost` or the reverse-proxy routes, no database schema, no tests.
    This page is a design placeholder describing the intended scope so the module can be built
    to the same vertical-slice shape as the others.

## Intended scope

| In scope | Out of scope |
|---|---|
| Register an owned asset with a category (vehicle, property, equipment, …), acquisition date and cost | Investment instruments — that belongs to [Investing](investing.md) |
| Record recurring / one-off costs against an asset | Day-to-day cash movements — that belongs to [Budget](budget.md) |
| Track current estimated value (manual revaluation or depreciation) | Marketplace pricing / valuations feeds |
| Feed a consolidated net-worth view (assets − liabilities) | Loan / mortgage amortisation schedules (could be a later slice) |

## Proposed model (draft)

- **`PersonalAsset`** — aggregate root: `Name`, `AssetCategory`, `AcquisitionDate`,
  `AcquisitionCost`, `CurrentValue`, `OwnerUserId`, `IsDisposed` (`ISoftDeletableEntity`).
- **`AssetCost`** — entity under `PersonalAsset`: `Date`, `Description`, `Amount`,
  `CostType` (recurring / one-off), optional `RecurrenceMonths`.
- **`Revaluation`** — entity under `PersonalAsset`: `Date`, `NewValue`, `Reason`.
- **`AssetCategory`** — enumeration: `Vehicle`, `Property`, `Equipment`, `Other`.

## Proposed integration

- Emit `PersonalAssetRegistered` / `PersonalAssetDisposed` / `AssetCostRecorded` on a
  `andor-personal-assets-events` topic.
- A future *Net Worth* view (or the Budget module) could consume these to combine account
  balances, investment positions and asset values.

## Building the module

Follow the seven-project vertical slice used by Budget and Communications:

```
Src/PersonalAssets/
  Andor.PersonalAssets.Domain
  Andor.PersonalAssets.Application      (Manager/Actor/Stash per aggregate)
  Andor.PersonalAssets.Contracts        (*Input / *Output)
  Andor.PersonalAssets.Infrastructure   (EF Core, schema "PersonalAssets", outbox)
  Andor.PersonalAssets.Binder           (Ioc + AkkaModule + UsePersonalAssets extension)
  Andor.PersonalAssets.RestApi          (controllers + UseApi())
  Andor.PersonalAssets.Service          (Program.cs)
Tests/PersonalAssets/Andor.PersonalAssets.Domain.Tests
```

Then register `personal-assets-service` in
`Src/Administrations/Aspire/Andor.AppHost/AppHost.cs` and add a `/personal-assets/*` route to
`Src/Administrations/ReverseProxy/Andor.Admin.ReverseProxy.Yarp/appsettings.json`.

## Next steps

- Confirm the scope and the model above with product.
- Decide whether depreciation is automatic (straight-line per category) or manual revaluation only.
- Scaffold the slice and a first `POST /v1/personal-asset` endpoint.
