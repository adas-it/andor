# 11. Risks & Technical Debt

Known weak spots, honestly listed. Severity is the author's estimate of impact × likelihood for
the project's current scope (personal / portfolio).

## 11.1 Risks

| ID | Risk | Severity | Mitigation / plan |
|---|---|---|---|
| R-1 | **Actors are single-node and in-memory.** No Akka persistence or clustering — a running actor's un-flushed state is lost on crash, and horizontal scaling of a write-heavy module would put two actor instances behind one aggregate id. | Medium | Writes are persisted per command (state isn't held dirty across commands), so crash loss is bounded. Before scaling out a module, add `Akka.Cluster.Sharding` or move the aggregate to persistent actors. |
| R-2 | **At-least-once delivery meets not-yet-idempotent consumers.** The Outbox + Service Bus guarantee redelivery; not every consumer has an explicit dedupe key. | Medium | Audit each `BackgroundService` consumer; add idempotency keys / upserts. Covered by scenario [QS-C4](10-quality-requirements.md#correctness-consistency). |
| R-3 | **Migrations applied on startup.** Two instances starting together, or a bad migration, can race or wedge a rollout. | Medium | Acceptable at one replica. For multi-replica, move migrations to an init job / release step. |
| R-4 | **No verification-code expiry in Onboarding.** A code is only invalidated by a new `start`; there is no TTL. | Low–Medium | Add a time-based expiry and rate-limiting on `start`. Tracked on the [Onboarding page](../onboarding.md). |
| R-5 | **Health endpoints mapped only in Development.** `MapDefaultEndpoints` guards `/health` + `/alive` behind `IsDevelopment()`, so a production orchestrator has nothing to probe. | Medium | Expose them in all environments with appropriate auth/network scoping before a real deployment. |
| R-6 | **`users-api` has no health check** registered in the AppHost, unlike the other services. | Low | Add `.WithHttpHealthCheck("/health")` and a readiness check. |
| R-7 | **Secrets via configuration.** SMTP and Service Bus credentials come from `appsettings`/env; no vault integration yet. | Medium | Wire Azure Key Vault / user-secrets; keep `EmailTest` set in non-prod to avoid real sends. |
| R-8 | **Single maintainer.** Bus factor of one; conventions live partly in `CLAUDE.md` and reviewers' heads. | Low (portfolio) | This arc42 set + ADRs are the mitigation. |

## 11.2 Technical debt

| ID | Debt | Where | Impact | Suggested fix |
|---|---|---|---|---|
| D-1 | **Two divergent CD paths.** `images.yml` (per-service → ACR) is current; `cd.yaml` (Docker Hub + Kustomize bump + Azure Web App) is legacy and mis-wired — its `workflows: ["CI build"]` never matches `build.yml`'s `name: .NET`. | `.github/workflows/` | Confusing; dead pipeline. | Delete `cd.yaml` or rewrite it against the real workflow name and the ACR images. |
| D-2 | **Stale k8s manifests.** `k8s/` describes one `andor-familybudget` container (port 80 / target 5000, Docker Hub image) — it predates the multi-service, port-8080, ACR model. | `k8s/deployment.yaml`, `service.yaml`, `kustomization.yaml` | Cannot deploy the current system as-is. | Regenerate manifests: one Deployment+Service per service, port 8080, ACR image refs, plus the reverse proxy. |
| D-3 | **Seed data quirk in Communications.** The `onboarding-verification-code` rule carries a template whose `Title` is `wellcome`; the `wellcome` rule itself is configured but unwired. | `Andor.Communications.Infrastructure/Migrations` | Works only because the Onboarding consumer requests `TemplateTitle = "wellcome"`; fragile and surprising. | Rename the template title to something like `verification-code`; wire the `wellcome` rule to a post-verify flow. |
| D-4 | **`Src/Goals` and Personal Assets are documented but not yet full slices.** The docs describe the domains; the solution folders/projects are partial. | `Andor.slnx` (`/Src/Goals/` empty folder), `docs/` | Docs promise more than the code delivers. | Either scaffold the seven-project slice or mark the pages "planned". |
| D-5 | **Contracts for onboarding endpoints undocumented.** `StartSignupInput` / `VerifySignupInput` shapes aren't in the docs. | `docs/onboarding.md` | Integrators must read code. | Add request/response tables. |
| D-6 | **No architecture fitness function.** Layering rules (no cross-slice `Domain` refs, domain has no framework refs) are enforced only by review. | build | Drift is possible. | Add an `ArchUnitNET` / `NetArchTest` test per module. |
| D-7 | **`codehilite` + `pymdownx` both enabled** in `mkdocs.yml`. Minor redundancy. | `docs/mkdocs.yml` | Cosmetic. | Drop `codehilite`; rely on `pymdownx.highlight`. |

## 11.3 Deliberately deferred

Not debt — conscious scope cuts for now:

- First-party UI, BI/reporting, and multi-currency FX rates.
- Full RBAC UI (permissions exist in the domain; no admin surface).
- Blue/green or canary deploys.
