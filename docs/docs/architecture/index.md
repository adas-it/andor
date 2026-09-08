# Architecture (arc42)

This section documents the architecture of **Andor** following the
[arc42](https://arc42.org) template. Each chapter answers a specific question; read them in order
for a full picture, or jump to the one you need.

| # | Chapter | Question it answers |
|---|---|---|
| 1 | [Introduction & Goals](01-introduction-and-goals.md) | What is Andor, who are the stakeholders, what are the top three quality goals? |
| 2 | [Architecture Constraints](02-constraints.md) | Which technical and organizational rules were non-negotiable? |
| 3 | [Context & Scope](03-context-and-scope.md) | What is inside the system, and which external systems does it talk to? |
| 4 | [Solution Strategy](04-solution-strategy.md) | What are the fundamental decisions that shape the whole design? |
| 5 | [Building Block View](05-building-block-view.md) | How is the code structured, from system down to a single slice? |
| 6 | [Runtime View](06-runtime-view.md) | How do the building blocks collaborate in the key scenarios? |
| 7 | [Deployment View](07-deployment-view.md) | How is the software mapped onto infrastructure? |
| 8 | [Cross-cutting Concepts](08-crosscutting-concepts.md) | Which patterns and rules apply everywhere (Result, actors, outbox, tenancy, auth, observability)? |
| 9 | [Architecture Decisions](09-architecture-decisions.md) | Which decisions are significant enough to record, and why? |
| 10 | [Quality Requirements](10-quality-requirements.md) | What does "good enough" mean, concretely, as testable scenarios? |
| 11 | [Risks & Technical Debt](11-risks-and-technical-debt.md) | What do we know is fragile or unfinished? |
| 12 | [Glossary](12-glossary.md) | What do the domain and technical terms mean? |

The detailed rationale behind individual choices lives in the
[**Architecture Decision Records**](adr/index.md).

!!! note "Status"
    Andor is a personal / portfolio project. The documentation describes the architecture **as
    built**; open items are called out explicitly in
    [chapter 11](11-risks-and-technical-debt.md).
