# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Andor is a modular-monolith personal-finance/investment platform (budget accounts, investment assets, communications/notifications, user identity & authorization) built on .NET 10, EF Core, Akka.NET actors, and .NET Aspire for local orchestration.

## Build & test commands

```bash
# Build everything
dotnet build Andor.slnx

# Run a single module's tests (pattern used by CI, per module)
dotnet test Tests/Budget/Andor.Accounts.Domain.Tests/Andor.Accounts.Domain.Tests.csproj
dotnet test Tests/Assets/Andor.Assets.Domain.Tests/Andor.Assets.Domain.Tests.csproj
dotnet test Tests/Administrations/Users/Andor.Users.Domain.Tests/Andor.Users.Domain.Tests.csproj

# Run a single test by name (xUnit)
dotnet test Tests/Budget/Andor.Accounts.Domain.Tests/Andor.Accounts.Domain.Tests.csproj --filter "FullyQualifiedName~AccountNewAsyncTests"

# Run the full local system (all services via Aspire AppHost)
dotnet run --project Src/Administrations/Aspire/Andor.AppHost/Andor.AppHost.csproj

# EF Core migrations (per-module DbContext, run from the module's Binder/Infrastructure project)
dotnet tool install --global dotnet-ef
```

CI (`.github/workflows/build.yml`) builds and tests each domain module independently (Accounts, Assets, Users) against SonarCloud, matrix-style — there is no single top-level "test everything" CI job, so when validating a change, run tests scoped to the module(s) you touched.

There is no lint/format step wired into CI; follow `.editorconfig` conventions already in the touched files.

## Architecture

### Module layout (vertical slices)

Each business capability under `Src/` (e.g. `Assets`, `Budget`/Accounts, `Communications`) is a **vertical slice** split into the same fixed set of projects:

- `*.Domain` — aggregates, value objects, domain events, repository interfaces (`ICommands*Repository`), validators. No framework dependencies. Errors are `DomainErrorCode`/`Notification` based.
- `*.Application` — Akka.NET actors and command handlers that orchestrate the domain (see "Actor model" below), plus `*CommandsService`/`*QueriesService` and their interfaces.
- `*.Contracts` — DTOs (`*Input`/`*Output`) exposed at the REST boundary, independent of Domain types.
- `*.Infrastructure` — EF Core `DbContext`, repository implementations, entity configs.
- `*.Binder` — composition root for the slice: `*Ioc.cs` (DI registration), `*AkkaModule.cs` (actor system wiring), and a single `Use<Module>Extensions.cs` that a `Service`/`WebApi` entry point calls to wire the whole slice into `WebApplicationBuilder`.
- `*.RestApi` — ASP.NET Core controllers + `ApiExtensions.UseApi()` to register the controller assembly part.
- `*.Service` (or `*.WebApi`) — the deployable entry point (`Program.cs`) that composes Aspire defaults, Swagger, JWT auth, and calls `builder.Use<Module>(...)`.

New modules should follow this same seven-project shape. When extending an existing module, mirror the existing file placement inside that shape rather than inventing a new structure.

### Shared/Foundation layer (`Src/Shared`, `Src/Foundation`)

Cross-cutting building blocks every module's `*.Domain`/`*.Application`/etc. depend on:

- `Andor.Foundation.Domain` — `SeedWork/` base types (`Entity<TId>`, `AggregateRoot<TId>` which raises `DomainEvent`s into an `Events` collection, `ICommandRepository<TEntity,TId>`), `DomainResult`/`Notification` result pattern, common value objects (`Name`, `Description`, `Month`, `Year`, `StringValueObject`), `Enumeration`.
- `Andor.Foundation.Application` — `ApplicationResult<T>` wraps application-layer outcomes (implicit conversion from `T`, accumulates errors/warnings/infos), `ICommands<TId>` marker for actor commands, `ITenantService`/`TenantService`, generic search (`SearchInput`/`SearchOutput`).
- `Andor.Foundation.Contracts` — the wire-level `ApplicationResult<T>` counterpart (`ErrorModel`, `DefaultResponse<T>`, `PaginatedListOutput`) used by controllers.
- `Andor.Foundation.Api` — `BaseController` with a single `Result<T>(ApplicationResult<T>)` helper that maps result state to `200/204/400/404/500` with a consistent `DefaultResponse<T>` envelope (including `TraceId` from `Activity.Current`). All controllers should extend this instead of hand-rolling status-code logic.
- `Andor.Foundation.Infrastructure` — EF Core helpers (`GlobalQueryExtensions`, `TenantDbContextExtensions`, `QueryHelper`), the **Outbox** implementation (`OutboxDispatcher` — a module-agnostic `BackgroundService` that polls every registered `IOutboxContextProvider` and publishes pending `OutboxMessage`s), and **Azure Service Bus** messaging (`ServiceBusIoc.WithAzureServiceBusMessaging`, `MessageSenderInterface`).
- `Andor.Foundation.ServerServices` — cross-service ASP.NET middleware (`GlobalExceptionHandlerMiddleware`, CORS, JSON provider).
- `AuthorizationDomain` (`Andor.Authorizations.Domain`/`.Application`) — `ICurrentUserService`/`CurrentUserService`, `IAuthorizationService`, `Permissions`, license types. Controllers pull the acting user via `ICurrentUserService`, not `HttpContext` directly.

Domain/value-object errors follow a `Result` pattern throughout, not exceptions: `DomainResult` (domain layer) → `ApplicationResult<T>` (application layer, carries `Data`) → `DefaultResponse<T>` (HTTP layer, via `BaseController.Result`). Preserve this chain when adding new use cases rather than throwing for expected failure paths.

### Actor model (Akka.NET) for aggregate command handling

Command-side writes to aggregates go through Akka.NET actors, not directly through services:

- A `*ManagerActor` (e.g. `AreaManagerActor`, `RuleManagerActor`) is the entry point per aggregate type. It receives a command implementing `ICommands<TId>`, derives a child actor name from the command's aggregate id, and forwards the command to that child, creating it on first use (`Context.Child` / `Context.ActorOf`).
- A `*Actor` (e.g. `AreaActor`) owns a single aggregate instance's lifecycle as a state machine using `Become`/`IWithUnboundedStash`: it starts in a `Loading` state (stashes incoming commands, self-sends a preload message, loads the aggregate from its `ICommandsRepository` via a fresh DI scope), then transitions to `Ready` and unstashes. `CreateAreaCommand`-style creation commands are handled in `Loading` directly (since there's nothing to load yet); subsequent mutation commands are only handled once `Ready`.
- Each module registers its actors via a `*Binder/Application/ApplicationAkkaModule.cs` implementing `IAkkaModule`, started by `builder.UseAkkaModules("<SystemName>")` in `Program.cs`.
- `*CommandsService` classes (application layer) are the façade application code/controllers call; internally they resolve the module's actor system and `Ask` the relevant `*ManagerActor`.

When adding a new aggregate command, follow this Manager→Actor→Stash pattern rather than calling the repository directly from the application service.

### Messaging: Outbox + Service Bus

Aggregates that need to publish integration events persist an `OutboxMessage` transactionally alongside the aggregate (see `Andor.Communications.Binder/Outbox/CommunicationOutboxContextProvider.cs` for the per-module wiring). The shared `OutboxDispatcher` background service polls all registered `IOutboxContextProvider`s and relays pending messages through `IMessageSenderInterface` (Azure Service Bus in production, configured via `ServiceBusIoc`). Don't publish to Service Bus directly from application code inside a request — write to the Outbox instead so publish is transactionally consistent with the DB write.

### Local orchestration

`Src/Administrations/Aspire/Andor.AppHost/AppHost.cs` is the single source of truth for which services exist and how they're wired together locally (configurations-service, users-api, assets-service, accounts-api, all fronted by a YARP reverse-proxy project). When adding a new deployable service, register it here too.

### Tests

xUnit + Moq, one test project per module domain (`Tests/<Area>/<Module>.Domain.Tests`), plus `Andor.TestsUtil` for shared generators (`GeneralFixture`). The Accounts tests (`Tests/Budget/Andor.Accounts.Domain.Tests`) are the most complete example: a `*Fixture` static class per aggregate builds valid instances via `Aggregate.NewAsync(...)` with a mockable validator, and test classes exercise one behavior per file (e.g. `AccountSoftDeleteTests`, `AccountAddTemplateCategoryTests`). Follow this Fixture + behavior-per-file convention for new domain tests rather than one large test class per aggregate.
