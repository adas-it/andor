# Andor

> A modular-monolith personal‑finance & investment platform built on **.NET 10**, **EF Core**, **Akka.NET** actors and **.NET Aspire**.

Andor is a personal project that manages the full picture of someone's finances — budget accounts, investment assets, personal assets, goals, user identity & authorization, and outbound communications — as a set of independently testable **vertical slices** inside a single deployable modular monolith.

📚 **Full documentation:** https://adas-it.github.io/andor/

## Quality Gates

Each business module is analyzed as its own project on SonarCloud (organization `adas-it`).

| Module | Quality Gate |
|---|---|
| Accounts / Budget | [![Quality gate status](https://sonarcloud.io/api/project_badges/measure?project=adas-it_andor-accounts&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=adas-it_andor-accounts) [Overview](https://sonarcloud.io/project/overview?id=adas-it_andor-accounts) |
| Investments / Assets | [![Quality gate status](https://sonarcloud.io/api/project_badges/measure?project=adas-it_andor-investments&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=adas-it_andor-investments) [Overview](https://sonarcloud.io/project/overview?id=adas-it_andor-investments) |
| Communications | [![Quality gate status](https://sonarcloud.io/api/project_badges/measure?project=adas-it_andor-communications&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=adas-it_andor-communications) [Overview](https://sonarcloud.io/project/overview?id=adas-it_andor-communications) |
| Configurations | [![Quality gate status](https://sonarcloud.io/api/project_badges/measure?project=adas-it_andor-configurations&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=adas-it_andor-configurations) [Overview](https://sonarcloud.io/project/overview?id=adas-it_andor-configurations) |
| Onboarding | [![Quality gate status](https://sonarcloud.io/api/project_badges/measure?project=adas-it_andor-onboarding&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=adas-it_andor-onboarding) [Overview](https://sonarcloud.io/project/overview?id=adas-it_andor-onboarding) |
| Users / Identity | [![Quality gate status](https://sonarcloud.io/api/project_badges/measure?project=adas-it_andor-users-identity&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=adas-it_andor-users-identity) [Overview](https://sonarcloud.io/project/overview?id=adas-it_andor-users-identity) |

## Highlights

- **Modular monolith / vertical slices** — every capability is split into the same fixed set of projects (`Domain`, `Application`, `Contracts`, `Infrastructure`, `Binder`, `RestApi`, `Service`) and can be built, tested and analyzed on its own.
- **Domain-Driven Design** — aggregates, value objects and domain events with no framework dependencies in the domain layer.
- **Result pattern end to end** — `DomainResult` → `ApplicationResult<T>` → `DefaultResponse<T>`; expected failures are values, not exceptions.
- **Actor-based write model** — aggregate commands flow through Akka.NET `Manager → Actor → Stash` state machines instead of hitting repositories directly.
- **Transactional Outbox + Azure Service Bus** — integration events are persisted alongside the aggregate and relayed by a shared background dispatcher.
- **.NET Aspire** for local orchestration of every service behind a YARP reverse proxy, with OpenTelemetry wired in.
- **Multi-tenant** aware persistence with EF Core global query filters.
- **OpenIddict / JWT** authentication and a dedicated authorization domain (permissions, licenses, current-user service).

## Tech Stack

| Area | Technology |
|---|---|
| Runtime | .NET 10, C# (nullable + implicit usings) |
| Persistence | EF Core 10, SQL Server |
| Actors | Akka.NET + Akka.Hosting |
| Messaging | Transactional Outbox, Azure Service Bus |
| Identity | OpenIddict 7, JWT Bearer |
| Orchestration | .NET Aspire, YARP reverse proxy |
| Observability | OpenTelemetry (traces, metrics, runtime) |
| API docs | Swagger / Swashbuckle, API versioning |
| Tests | xUnit, Moq, component tests via `WebApplicationFactory` |
| CI / Quality | GitHub Actions, SonarCloud, dotnet-coverage |
| Delivery | Docker (single reusable image), Kubernetes (Kustomize), Azure Container Registry |
| Docs site | MkDocs Material → GitHub Pages |

## Modules

| Module | Responsibility |
|---|---|
| **Budget / Accounts** | Personal financial control: current accounts, transactions, budgets, category templates. |
| **Assets / Investments** | Investment tracking, FIFO calculation for taxes, analysis. |
| **Personal Assets** | Physical goods (car, house, bicycle) and their related expenses. |
| **Goals** | Personal goals and projects, linked to financial transactions. |
| **Onboarding** | Public, anonymous signup flow: start (name + e-mail → verification code) and verify (code + password), which fans out events to Users and Accounts. |
| **Communications** | The single place where outbound notifications (e-mail today) are rendered and delivered, driven by configurable **Rules** and **Templates**. |
| **Users / Identity** | User identity and account management. |
| **Configurations** | Cross-cutting runtime configuration service. |

## Architecture

### Vertical slice shape

Every module under `Src/` is built from the same seven projects:

| Project | Contains |
|---|---|
| `*.Domain` | Aggregates, value objects, domain events, repository interfaces, validators. No framework dependencies. |
| `*.Application` | Akka.NET actors, command/query services that orchestrate the domain. |
| `*.Contracts` | `*Input` / `*Output` DTOs for the REST boundary. |
| `*.Infrastructure` | EF Core `DbContext`, repository implementations, entity configs. |
| `*.Binder` | Composition root: DI registration, Akka module wiring, one `Use<Module>` extension. |
| `*.RestApi` | ASP.NET Core controllers extending a shared `BaseController`. |
| `*.Service` / `*.WebApi` | Deployable entry point (`Program.cs`) composing Aspire defaults, Swagger, JWT and the module. |

### Shared foundation

`Src/Shared` and `Src/Foundation` provide the cross-cutting building blocks: `SeedWork` base types (`Entity<TId>`, `AggregateRoot<TId>`), the `Notification` / result pattern, common value objects, the `BaseController` HTTP envelope, EF Core helpers, the Outbox implementation, Service Bus messaging, JWT authentication and the authorization domain.

### Command handling with actors

Writes to aggregates go through Akka.NET:

1. A `*ManagerActor` receives a command (`ICommands<TId>`), derives a child actor name from the aggregate id and forwards it, creating the child on first use.
2. A `*Actor` owns one aggregate instance as a `Become` / `IWithUnboundedStash` state machine: it starts in `Loading` (stashes commands, loads the aggregate from its repository in a fresh DI scope), then flips to `Ready` and unstashes.
3. `*CommandsService` classes are the façade the controllers call; internally they `Ask` the relevant manager.

### Messaging

Aggregates that publish integration events persist an `OutboxMessage` in the same transaction as the aggregate. The shared `OutboxDispatcher` background service polls every registered `IOutboxContextProvider` and relays pending messages through Azure Service Bus — so publishing stays consistent with the database write.

## Project Structure

```
Andor.slnx
Src/
  Administrations/
    Aspire/            # AppHost (local orchestration) + ServiceDefaults
    Configurations/    # configurations-service
    Users/             # users-api (WebApi)
    ReverseProxy/      # YARP reverse proxy
  Assets/              # assets-service
  Budget/              # accounts-api
  Communications/      # communications-api
  Onboarding/          # onboarding-api
  Shared/              # Foundation.*, Authentication.Jwt, AuthorizationDomain, ...
Tests/
  <Area>/<Module>.Domain.Tests
  <Area>/<Module>.Infrastructure.Tests
  <Area>/<Module>.ComponentTests
docs/                  # MkDocs Material source -> GitHub Pages
k8s/                   # Kustomize manifests
Dockerfile             # one reusable image for every deployable service
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Docker (used by Aspire for SQL Server and supporting containers)
- Optional: `dotnet tool install --global dotnet-ef` for migrations

### Run the whole system locally

```bash
dotnet run --project Src/Administrations/Aspire/Andor.AppHost/Andor.AppHost.csproj
```

This starts every service (configurations, users, assets, accounts, communications, onboarding) behind the YARP reverse proxy on `https://localhost:7000`, with the Aspire dashboard for logs, traces and metrics.

### Build

```bash
dotnet build Andor.slnx
```

### Test

Tests are scoped per module — run the ones for the module you touched:

```bash
dotnet test Tests/Budget/Andor.Accounts.Domain.Tests/Andor.Accounts.Domain.Tests.csproj
dotnet test Tests/Assets/Andor.Assets.Domain.Tests/Andor.Assets.Domain.Tests.csproj
dotnet test Tests/Administrations/Users/Andor.Users.Domain.Tests/Andor.Users.Domain.Tests.csproj
```

Run a single test by name:

```bash
dotnet test Tests/Budget/Andor.Accounts.Domain.Tests/Andor.Accounts.Domain.Tests.csproj --filter "FullyQualifiedName~AccountNewAsyncTests"
```

Domain tests follow a **Fixture + behavior-per-file** convention: a `*Fixture` static class builds valid aggregates via `Aggregate.NewAsync(...)` with a mockable validator, and each test class exercises one behavior.

## Containers & Deployment

A single `Dockerfile` builds every deployable service — the service is selected with build args:

```bash
docker build \
  --build-arg PROJECT=Src/Budget/Andor.Accounts.Service/Andor.Accounts.Service.csproj \
  --build-arg APP_DLL=Andor.Accounts.Service.dll \
  -t andor/accounts-api .
```

Kestrel listens on `8080` (plain HTTP); TLS is terminated by the platform ingress. Kubernetes manifests live in `k8s/` and are managed with Kustomize.

## CI / CD

| Workflow | Trigger | Does |
|---|---|---|
| `.github/workflows/build.yml` | push / PR to `main`, `v*` tags | Matrix build + test of each module against SonarCloud with coverage. |
| `.github/workflows/images.yml` | after a green `.NET` run on `main`, `v*` tags | Builds and pushes each service image to Azure Container Registry. |
| `.github/workflows/docs.yml` | changes under `docs/**` | Builds the MkDocs site and deploys it to GitHub Pages. |

There is no single "test everything" job by design — each domain module is validated independently.

## Documentation

The design docs (domain descriptions, UML diagrams, sequence flows, business rules) are published from the `docs/` folder to GitHub Pages:

**https://adas-it.github.io/andor/**
