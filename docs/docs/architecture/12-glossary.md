# 12. Glossary

## Domain terms

| Term | Meaning |
|---|---|
| **Account** | A budget/current account in the Budget module: the container for financial movements, categories and budgets. Can be shared via an **Invite**. |
| **Financial movement** | A single credit/debit entry against an account, with a type, status and category. |
| **Cash-flow projection** | A forward-looking view of an account's balance, maintained by a consumer in `accounts-api`. |
| **Invite** | A request to share an account with another person. |
| **Asset (investment)** | An investment instrument tracked in the Assets/Investing module; positions are cost-tracked with **FIFO**. |
| **FIFO** | *First-In-First-Out* cost-basis method used to compute realised cost/gain for tax purposes. |
| **Personal asset** | A physical good (car, house, bicycle) and its running costs. |
| **Goal** | A personal objective/project, optionally linked to financial movements. |
| **Signup request** | An in-progress onboarding, identified by `SignupRequestId`, moving through `start` → `verify`. |
| **Verification code** | 6-digit code e-mailed during `start`; invalidated by a new `start` (no TTL today). |
| **Rule** (*régua*) | Communications aggregate root: a named slot for one kind of message (e.g. `onboarding-verification-code`). Owns many **Templates**. |
| **Template** | The renderable content of a message: `Subject` + `Value` with `<token>` placeholders, keyed by `Title` + `ContentLanguage`, delivered by a `Partner`. |
| **Partner** | Delivery handler for a template: `InHouse` (SMTP) today; `SendGrid` reserved. |
| **Tenant** | Isolation unit for data; the active tenant selects the connection string and scopes every query. |

## Architecture / technical terms

| Term | Meaning |
|---|---|
| **Modular monolith** | One codebase and deployable unit family with enforced internal module boundaries; see [ADR-0001](adr/0001-modular-monolith.md). |
| **Vertical slice** | A business capability implemented top-to-bottom as its own set of projects, rather than a horizontal technical layer. |
| **Slice shape / seven projects** | `*.Domain`, `*.Application`, `*.Contracts`, `*.Infrastructure`, `*.Binder`, `*.RestApi`, `*.Service` — see [§5.2](05-building-block-view.md). |
| **Aggregate root** | DDD consistency boundary; `AggregateRoot<TId>` raises `DomainEvent`s into an `Events` collection. |
| **Value object** | Immutable, self-validating domain type (`Name`, `Month`, typed ids…). |
| **DomainResult / Notification** | Domain-layer outcome type; carries error codes instead of throwing. |
| **ApplicationResult\<T\>** | Application-layer outcome wrapper: `Data` + accumulated errors/warnings/infos. |
| **DefaultResponse\<T\>** | HTTP response envelope (`data`, `errors`, `traceId`) produced by `BaseController.Result`. |
| **BaseController** | Shared controller base; single place that maps `ApplicationResult` to `200/204/400/404/500`. |
| **ManagerActor** | Akka.NET actor that routes a command to the child actor for its aggregate id. |
| **Aggregate actor (`*Actor`)** | Akka.NET actor owning one aggregate instance; `Loading → Ready` with stash. |
| **Stash** | Akka.NET mechanism to hold messages received in the wrong state and replay them after a transition. |
| **Outbox / OutboxMessage** | A row persisted in the same transaction as an aggregate write, holding an integration event to be published later. |
| **OutboxDispatcher** | Module-agnostic `BackgroundService` that polls every `IOutboxContextProvider` and relays pending messages. |
| **IOutboxContextProvider** | Per-module hook that exposes that module's pending `OutboxMessage`s to the dispatcher. |
| **IMessageSenderInterface** | Abstraction over the message transport (Azure Service Bus in production). |
| **Consumer** | Per-service `BackgroundService` that reads a queue/subscription and `Complete`s or `Abandon`s messages. |
| **Integration event** | A fact published by one module for others to react to asynchronously (e.g. `SignupVerifiedDomainEvent`). |
| **Domain event** | An in-process event raised by an aggregate; may become an integration event via the outbox. |
| **ServiceDefaults** | Shared host extension (`AddServiceDefaults`) adding OpenTelemetry, health checks, service discovery and HTTP resilience. |
| **AppHost** | The .NET Aspire project that declares and wires all local services. |
| **Reverse proxy (YARP)** | Single ingress; path-prefix route per module with `PathRemovePrefix`. |
| **Binder** | A slice's composition root: DI registration, Akka module, `Use<Module>` extension. |
| **Central Package Management** | NuGet feature pinning every package version once in `Directory.Packages.props`. |
| **Global query filter** | EF Core predicate applied to every query for an entity type; used here for tenant scoping. |
| **arc42** | The template this documentation follows. |
| **ADR** | *Architecture Decision Record* — a one-page record of a significant decision and its consequences; see [adr/](adr/index.md). |

## Service ↔ module map

| Deployable service | Slice(s) | Proxy prefix | Actor system |
|---|---|---|---|
| `accounts-api` | Budget / Accounts | `/accounts` | `AndorAccountsSystem` |
| `assets-service` | Assets / Investing | `/assets` | *(module system)* |
| `users-api` | Users / Identity | `/users` | *(module system)* |
| `onboarding-api` | Onboarding | `/onboarding` | *(module system)* |
| `communications-api` | Communications | `/communications` | *(module system)* |
| `configurations-service` | Configurations | `/configurations` | *(module system)* |
| `reverse-proxy` | — (ingress) | `:7000` | — |
