# Communications Domain

## Description

The Communications domain is the single place where **outbound notifications** (today: e-mail) are
rendered and delivered. Any other bounded context (Onboarding, Users/Identity, Accounts, …) that
needs to talk to a user does **not** build the message itself — it asks Communications to do it,
referencing a pre-configured **Rule** and **Template** by id/name and passing only the dynamic
values.

This keeps message wording, subjects, languages and delivery channels out of the business modules
and centralised behind a small, stable contract.

## Domain model

### Rule (`RuleId`) — the "régua"

A **Rule** (in Portuguese, *régua*) is the aggregate root. Think of it as a **named slot for one
kind of message** — e.g. "the onboarding verification code message" or "the welcome message".

| Field | Notes |
|---|---|
| `Id` (`RuleId`) | `uniqueidentifier`. This is the value other modules store and send as `RuleId`. Stable — it is what everything keys off. |
| `Name` | Human-readable identifier, max 70 chars (e.g. `onboarding-verification-code`). |
| `Type` | Enumeration: `0 = Undefined`, `1 = Information`, `2 = Marketing`. |
| `CreatedAt` | UTC. |
| `Templates` | Collection of `Template` — **one Rule has many Templates**. |

### Template

A Rule owns **one or more Templates**. The Template is the actual content that gets rendered and
sent. Having many templates per rule lets the same logical message vary by **language**, **title
variant** and **delivery partner** without creating a new Rule.

| Field | Notes |
|---|---|
| `Id` (`TemplateId`) | `uniqueidentifier`. |
| `RuleId` | FK back to the owning Rule (`Communication.Rule.Id`), `ON DELETE CASCADE`. |
| `Value` | The body. `nvarchar(max)`. Contains placeholder tokens (see below). |
| `ContentLanguage` | e.g. `en`, `pt`. Max 10 chars. Part of the lookup key. |
| `Title` | Short template identifier, max 50 chars. Part of the lookup key. |
| `Subject` | E-mail subject, max 50 chars. Placeholders are substituted here too. |
| `Partner` | Enumeration: `0 = Undefined`, `1 = InHouse` (SMTP), `2 = SendGrid`. Selects the delivery handler. |
| `IsDefault` | Marks the fallback template for the rule. |
| `CreatedAt` | UTC. |

### How a template is chosen at send time

When a notification is requested, `RuleActor` loads the Rule (with its templates) and picks the
template where:

```
Template.Title == request.TemplateTitle  &&  Template.ContentLanguage == request.ContentLanguage
```

If no template matches, the request fails with `RuleNotFound` (5001). The chosen template's
`Partner` decides which handler sends it (`InHousePartner` → SMTP today).

### Placeholder substitution

`Value` and `Subject` are plain strings with literal tokens. The caller passes a
`Dictionary<string,string>` of `token → replacement` and the partner handler does a straight
`string.Replace` for every entry, on both body and subject.

Example body: `<h1>Hello <name>!</h1><br>Your code is <code>`
Example `Values`: `{ "<name>": "Ada", "<code>": "123456" }`

## Seeded configuration (already in the database)

Two migrations seed this data, applied automatically on service start-up by
`app.ApplyCommunicationMigrationsAsync()`:

- [`20260902110428_SeedInitialRulesAndTemplates`](https://github.com/adas-it/andor/blob/main/Src/Communications/Andor.Communications.Infrastructure/Migrations/20260902110428_SeedInitialRulesAndTemplates.cs)
  — initial seed with placeholder/dummy template content.
- [`20260917101701_SeedRealTemplates`](https://github.com/adas-it/andor/blob/main/Src/Communications/Andor.Communications.Infrastructure/Migrations/20260917101701_SeedRealTemplates.cs)
  — replaces the dummy `onboarding-verification-code` template with the real HTML from
  `Src/Communications/templates/01-codigo-verificacao.html`, adds two new rules/templates from
  `02-boas-vindas.html` and `03-convite-conta.html`, and removes the orphaned `wellcome` rule
  (`42b2aa27-…`) that the first migration seeded but no producer ever targeted. All three land
  tagged `ContentLanguage = "en"` even though the copy is Portuguese — a mislabel fixed below.
- [`20260922182122_SeedMultilingualTemplates`](https://github.com/adas-it/andor/blob/main/Src/Communications/Andor.Communications.Infrastructure/Migrations/20260922182122_SeedMultilingualTemplates.cs)
  — relabels those three templates' `ContentLanguage` from `en` to `br` (the ISO code
  `Andor.Shared.Lookups.Language` uses for Portuguese — see `Recipient.PreferredLanguageId`), and
  adds real `en` and `es` translations for each of the three rules, so the Title+ContentLanguage
  lookup can actually serve all three languages instead of only ever matching the (mislabeled)
  Portuguese copy.

### Rules

| `Id` | `Name` | `Type` |
|---|---|---|
| `acb860a5-1af6-4b03-afae-e290dfcac7d4` | `onboarding-verification-code` | `1` (Information) |
| `875725eb-683a-4f33-b27f-32489d127e4b` | `welcome-after-verification` | `1` (Information) |
| `ef680a94-c366-4d8a-92a4-65ed8c8fc807` | `account-invite` | `1` (Information) |

### Templates

Each rule now has three templates — one per `ContentLanguage` (`en`, `br`, `es`) — all sharing the
same `Title = "wellcome"` lookup key (`RuleActor`/`SendCommunicationFunction` match on
`RuleId + Title + ContentLanguage`).

| `RuleId` | `en` | `br` (Portuguese) | `es` |
|---|---|---|---|
| `acb860a5-…dfcac7d4` (`onboarding-verification-code`) | `1e0a3b4c-…4e01` — "Your Berry verification code" | `a382b4e7-db3d-49a8-a15b-c258e86cd6d0` — "Seu código de verificação Berry" | `2e0a3b4c-…4e02` — "Tu código de verificación de Berry" |
| `875725eb-…` (`welcome-after-verification`) | `1e0a3b4c-…4e03` — "Welcome to Berry" | `9f501ec1-0d01-4826-acb3-25592c4de98e` — "Bem-vindo ao Berry" | `2e0a3b4c-…4e04` — "Bienvenido a Berry" |
| `ef680a94-…` (`account-invite`) | `1e0a3b4c-…4e05` — "You've been invited to a Berry account" | `4bfcd0b7-59b3-4bd9-89db-424dcef74ccd` — "Você foi convidado para uma conta no Berry" | `2e0a3b4c-…4e06` — "Has sido invitado a una cuenta de Berry" |

All rows use `Partner = 1` (InHouse) and `IsDefault = true`. Tokens per rule: verification-code
uses `<code>`; welcome-after-verification uses `<name>`; account-invite uses `<inviter_name>`,
`<inviter_full_name>`, `<inviter_email>`, `<inviter_initials>`, `<account_name>`, `<access_level>`,
`<accept_url>`, `<decline_url>`.

!!! note "Title is always `wellcome`; `ContentLanguage` now genuinely varies"
    Every seeded template uses the literal `Title = "wellcome"` — that's not a naming mistake,
    it's the lookup key every producer sends. `ContentLanguage` used to be stuck at the literal
    `"en"` regardless of which language the HTML was actually written in; it now matches the real
    language of each row (`en`/`br`/`es`), so a producer that resolves the recipient's actual
    preferred language (via `Language.GetById(recipient.PreferredLanguageId).ISO`, currently
    `en`/`br`/`fr` — there's no Spanish entry in that lookup yet, so `es` templates are only
    reachable by a caller that passes `ContentLanguage: "es"` explicitly) gets the matching
    translation instead of always landing on the Portuguese copy.

!!! note "No producer yet for `account-invite`"
    `ef680a94-c366-4d8a-92a4-65ed8c8fc807` is seeded and ready, but nothing publishes a
    `SendNotificationInput` for it yet. Whichever module implements the "invite someone to a
    shared account" flow should send `Values` for all eight tokens listed above.

## How Communications is invoked

Requesting a communication is a **two-hop queue pipeline**, split across two processes so that
only Communications' own code is ever allowed to hand a message to a delivery partner:

```
Onboarding (or any module) --publish--> "request-communication" queue
                                              |
                                              v
                          Andor.Communications.Service: RequestCommunicationConsumer
                          (enriches from the Recipient projection, gates Marketing
                           sends on consent, via IRequestCommunicationService)
                                              |
                                              v
                                   "send-communication" queue
                                              |
                                              v
                    Andor.Communications.External: SendCommunicationFunction
                    (Azure Function; loads the Rule/Template and calls the
                     partner handler directly — SMTP today — bypassing the
                     Rule actor system entirely, since sending never mutates
                     the Rule aggregate)
```

### 1. `request-communication` — any module asks for a communication

Any module publishes onto the Azure Service Bus queue **`request-communication`**
(`ServiceBus:Queues:RequestCommunication` in the producer's `appsettings.json`, e.g. Onboarding's)
via `IMessageSenderInterface.QueueSendAsync`. The message body is JSON matching
`RequestCommunicationInput`:

```jsonc
{
  "RuleId": "acb860a5-1af6-4b03-afae-e290dfcac7d4",
  "TemplateTitle": "wellcome",
  "UserId": "…",               // or RecipientEmail + ContentLanguage when no User exists yet
  "RecipientEmail": null,
  "ContentLanguage": null,
  "Values": { "<code>": "123456" }
}
```

`Andor.Communications.Service` hosts `RequestCommunicationConsumer` (a `BackgroundService`,
`RequestCommunicationQueue:QueueName` in its own `appsettings.json`), which resolves the Recipient
by `UserId` when one is given (filling in Email/ContentLanguage/`<name>`), rejects Marketing sends
without consent, and republishes a `SendNotificationInput` onto `send-communication` via the
module's default `IMessageSenderInterface.QueueSendAsync` (`ServiceBus:QueueName`).

- On success → `CompleteMessageAsync` (message removed from the queue).
- On a domain failure (recipient/rule not found, missing consent, etc.) → logs the errors and
  `AbandonMessageAsync`, so Service Bus retries and eventually dead-letters the message.

**Real example — Onboarding.** When a signup code is generated, Onboarding emits
`SignupCodeGenerated`; its `SignupCodeGeneratedConsumer` publishes a `RequestCommunicationInput`
(with `RuleId = acb860a5-…`, `TemplateTitle = "wellcome"`, and `Values` `<code>` / `<name>`)
directly onto `request-communication`. Onboarding never knows the subject line, the wording or
that SMTP is used — and never calls Communications over HTTP.

Similarly, once the User is actually created, the Users module emits `UserCreatedDomainEvent` on
`andor-users-events`; Onboarding's `UserCreatedConsumer` publishes a `RequestCommunicationInput`
with `RuleId = 875725eb-…`, `TemplateTitle = "wellcome"` and just the `UserId` — matching the
`welcome-after-verification` rule/template above, enriched from the Recipient projection on the
Communications side. That projection is fed by `RecipientSyncConsumer`, which subscribes to the
same `UserCreatedDomainEvent` (not to Onboarding), so Communications' view of a user comes from
the module that owns it.

### 2. `send-communication` — dispatch only, Communications-owned

Nothing publishes onto `send-communication` except `RequestCommunicationConsumer` above — it is
not a public entry point. `SendCommunicationFunction` (in `Andor.Communications.External`, the
Azure Function project) is the sole consumer: it loads the Rule/Template and calls the matching
partner handler (`InHousePartner` → SMTP today) directly, since dispatch never mutates the Rule
aggregate and doesn't need the Manager/Actor/Stash write path.

### 3. Via REST (direct / operational use)

`CommunicationsController` (JWT-protected, `v{version}/Communications`):

| Method | Route | Purpose |
|---|---|---|
| `POST` | `/v1/Communications/rules` | Create a new Rule (optionally with inline templates). Body: `CreateRuleInput`. |
| `POST` | `/v1/Communications/notifications` | Send a notification now, through the actor-backed `RuleManagerActor` → `RuleActor` path (one child actor per `RuleId`). Body: `SendNotificationInput`. |

The REST `notifications` endpoint is mainly for testing and operational one-offs; production
traffic from other bounded contexts should go through the `request-communication` queue so
delivery is decoupled, retryable, and consent-gated.

## Configuration

`Andor.Communications.Service/appsettings.json`:

- `ServiceBus:QueueName` — the *outbound* queue `RequestCommunicationConsumer` publishes to
  (`send-communication`), consumed only by `SendCommunicationFunction`.
- `RequestCommunicationQueue:QueueName` — the *inbound* queue `RequestCommunicationConsumer`
  listens on (`request-communication`).
- `ServiceBus:FullyQualifiedNamespace` / `ConnectionString` — Service Bus connection.
- `ApplicationSettings:SmtpConfig` — SMTP host/port/credentials for `InHousePartner`.
  `EmailTest`, when set, redirects every outgoing e-mail to that address.
- `Tenants` — connection string(s) for the `Communication` schema database.

## Domain error codes

| Code | Name | When |
|---|---|---|
| 5000 | `ActionNotAllowed` | Operation not permitted for the current user. |
| 5001 | `RuleNotFound` | Rule id unknown, or no template matches `Title` + `ContentLanguage` in that rule. |
| 5002 | `SkippedValidations` | Informational — validations were bypassed (`Force`). |
| 5003 | `RuleValidation` | Rule/template failed validation on create. |

## Adding a new rule or template

Follow the seed migration pattern: create a new EF migration under
`Src/Communications/Andor.Communications.Infrastructure/Migrations` and use
`migrationBuilder.InsertData` against `Communication.Rule` / `Communication.Template` with fixed
GUIDs and a fixed `CreatedAt` (migrations must be deterministic). Insert the Rule row first, then
its Template rows (FK). Provide a matching `DeleteData` in `Down`.

```bash
dotnet ef migrations add <Name> \
  --project Src/Communications/Andor.Communications.Infrastructure/Andor.Communications.Infrastructure.csproj \
  --context CommunicationContext
```
