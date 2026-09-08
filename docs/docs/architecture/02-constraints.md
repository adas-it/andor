# 2. Architecture Constraints

Constraints are fixed conditions the architecture had to accept. They are not decisions — the
decisions ([chapter 4](04-solution-strategy.md), [ADRs](adr/index.md)) are made *within* these
limits.

## 2.1 Technical constraints

| ID | Constraint | Consequence |
|---|---|---|
| TC-1 | **.NET 10 / C#** on the whole backend. `Nullable` and `ImplicitUsings` enabled solution-wide (`Directory.Build.props`). | One language, one runtime; nullable reference types are part of the domain contract. |
| TC-2 | **EF Core 10** on **SQL Server** for persistence. | Relational modelling; each module owns its `DbContext`; schema changes go through EF migrations. |
| TC-3 | **Central Package Management** — every NuGet version is pinned once in `Directory.Packages.props`. | No per-project version drift; upgrades are a single-file change. |
| TC-4 | **.NET Aspire** is the local orchestration model; `Andor.AppHost` is the single source of truth for which services exist locally. | New deployable services must be registered in the AppHost and in the image build matrix. |
| TC-5 | **Akka.NET** is available as the actor runtime (`Akka.Hosting`). | The write model is expressed as actors; each module hosts its own `ActorSystem`. |
| TC-6 | **Azure Service Bus** is the production message transport; **Azure Container Registry** hosts images; **Azure Web App / Kubernetes** is the runtime target. | Messaging code is written against `IMessageSenderInterface`; delivery is queue-based and retryable. |
| TC-7 | **OpenIddict / JWT Bearer** for authentication; API versioning via `Asp.Versioning`. | Every service validates JWTs the same way; routes are `/v{version}/…`. |
| TC-8 | **Containerised deployment** — one reusable `Dockerfile` parameterised by build args selects the service. Kestrel listens on plain HTTP `8080`; TLS is terminated at the ingress. | Services must not assume they own TLS; configuration comes from the environment. |
| TC-9 | **OpenTelemetry** is the only telemetry API (traces, metrics, logs) via `Andor.ServiceDefaults`. | No direct vendor SDK calls in service code. |

## 2.2 Organizational & process constraints

| ID | Constraint | Consequence |
|---|---|---|
| OC-1 | **Single maintainer**, portfolio project. | Operational complexity must stay low → modular monolith, not microservices ([ADR-0001](adr/0001-modular-monolith.md)). |
| OC-2 | **CI is per-module** (`.github/workflows/build.yml` matrix). There is no "build & test everything" job. | Changes are validated against the module(s) touched; module boundaries must be real enough for that to be meaningful. |
| OC-3 | **SonarCloud quality gate per module** (`adas-it_andor-<module>`). | Each slice is independently measured for coverage, duplication, reliability, security. |
| OC-4 | **Docs are code** — MkDocs Material in `docs/`, published to GitHub Pages by `docs.yml` on changes under `docs/**`. Built with `--strict`. | Documentation lives with the source and breaks the build if links/nav are wrong. |
| OC-5 | No dedicated QA/ops team. | Health checks, structured telemetry and safe-by-default configuration are mandatory, not optional. |

## 2.3 Conventions

| Area | Convention |
|---|---|
| Code style | `.editorconfig` at repo root; follow the style already in the file being edited. No separate lint/format CI step. |
| Module shape | Every slice is the same seven projects — see [Building Block View §5.2](05-building-block-view.md). |
| Errors | `DomainResult` → `ApplicationResult<T>` → `DefaultResponse<T>`; never throw for an expected failure path. |
| Aggregate writes | `ManagerActor → Actor → Stash` load-then-serve pattern; no repository calls straight from an application service. |
| Integration events | Persist an `OutboxMessage` in the same transaction as the aggregate; never publish to Service Bus inline. |
| Tests | One test project per module domain; `Fixture` + behaviour-per-file (xUnit + Moq). |
