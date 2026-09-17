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
  (`42b2aa27-…`) that the first migration seeded but no producer ever targeted.

### Rules

| `Id` | `Name` | `Type` |
|---|---|---|
| `acb860a5-1af6-4b03-afae-e290dfcac7d4` | `onboarding-verification-code` | `1` (Information) |
| `875725eb-683a-4f33-b27f-32489d127e4b` | `welcome-after-verification` | `1` (Information) |
| `ef680a94-c366-4d8a-92a4-65ed8c8fc807` | `account-invite` | `1` (Information) |

### Templates

| `Id` | `RuleId` | `Title` | `Lang` | `Partner` | `IsDefault` | `Subject` | Source file |
|---|---|---|---|---|---|---|---|
| `a382b4e7-db3d-49a8-a15b-c258e86cd6d0` | `acb860a5-…dfcac7d4` (`onboarding-verification-code`) | `wellcome` | `en` | `1` (InHouse) | `true` | `Seu código de verificação Berry` | `01-codigo-verificacao.html` (tokens: `<code>`) |
| `9f501ec1-0d01-4826-acb3-25592c4de98e` | `875725eb-…` (`welcome-after-verification`) | `wellcome` | `en` | `1` (InHouse) | `true` | `Bem-vindo ao Berry` | `02-boas-vindas.html` (tokens: `<name>`) |
| `4bfcd0b7-59b3-4bd9-89db-424dcef74ccd` | `ef680a94-…` (`account-invite`) | `wellcome` | `en` | `1` (InHouse) | `true` | `Você foi convidado para uma conta no Berry` | `03-convite-conta.html` (tokens: `<inviter_name>`, `<inviter_full_name>`, `<inviter_email>`, `<inviter_initials>`, `<account_name>`, `<access_level>`, `<accept_url>`, `<decline_url>`) |

!!! note "Title is always `wellcome`, and `ContentLanguage` is always `en`"
    Every seeded template uses the literal `Title = "wellcome"` — that's not a naming mistake,
    it's the lookup key every producer sends (`RuleActor` matches on `RuleId + Title +
    ContentLanguage`). Likewise `ContentLanguage` stays `"en"` even though the actual HTML is
    Portuguese (`pt-BR`); changing either would break every existing producer, since they hardcode
    `TemplateTitle: "wellcome"` and `ContentLanguage: "en"` in their `SendNotificationInput`. Treat
    both as an established (if awkward) convention rather than something to "fix" in isolation.

!!! note "No producer yet for `account-invite`"
    `ef680a94-c366-4d8a-92a4-65ed8c8fc807` is seeded and ready, but nothing publishes a
    `SendNotificationInput` for it yet. Whichever module implements the "invite someone to a
    shared account" flow should send `Values` for all eight tokens listed above.

## How Communications is invoked

There are two entry points. Both end up in the same place:
`IRuleCommandsService.SendNotificationAsync` → `RuleManagerActor` → `RuleActor` (one child actor
per `RuleId`, so sends for the same rule are processed sequentially) → partner handler → SMTP.

### 1. Via the queue (the normal path for other modules)

The service hosts `RequestCommunicationConsumer` (a `BackgroundService`) which listens on the
Azure Service Bus queue **`request-communication`** (`ServiceBus:QueueName` in
`appsettings.json`).

Any module publishes a message to that queue via the shared Outbox / `IMessageSenderInterface`.
The message body is JSON matching `SendNotificationInput`:

```jsonc
{
  "RuleId": "acb860a5-1af6-4b03-afae-e290dfcac7d4",
  "RecipientEmail": "user@example.com",
  "TemplateTitle": "wellcome",
  "ContentLanguage": "en",
  "Values": {
    "<name>": "Ada",
    "<code>": "123456"
  }
}
```

Consumer behaviour:

- On success → `CompleteMessageAsync` (message removed from the queue).
- On a domain failure (rule/template not found, etc.) → logs the errors and `AbandonMessageAsync`,
  so Service Bus retries and eventually dead-letters the message per its own configuration.

**Real example — Onboarding.** When a signup code is generated, Onboarding emits
`SignupCodeGenerated`; its `SignupCodeGeneratedConsumer` translates that into a
`SendNotificationInput` (with `RuleId = acb860a5-…`, `TemplateTitle = "wellcome"`, and
`Values` `<code>` / `<name>`) and drops it on `request-communication`. Communications picks it up
and sends the e-mail. Onboarding never knows the subject line, the wording or that SMTP is used.

Similarly, when a signup is verified, Onboarding emits `SignupVerifiedDomainEvent`; its
`SignupVerifiedConsumer` sends a `SendNotificationInput` with `RuleId = 875725eb-…`,
`TemplateTitle = "wellcome"`, and `Values` `<name>` — matching the `welcome-after-verification`
rule/template above.

### 2. Via REST (direct / operational use)

`CommunicationsController` (JWT-protected, `v{version}/Communications`):

| Method | Route | Purpose |
|---|---|---|
| `POST` | `/v1/Communications/rules` | Create a new Rule (optionally with inline templates). Body: `CreateRuleInput`. |
| `POST` | `/v1/Communications/notifications` | Send a notification now. Body: `SendNotificationInput` (same shape as the queue message). |

The REST `notifications` endpoint is mainly for testing and operational one-offs; production
traffic from other bounded contexts should go through the queue so delivery is decoupled and
retryable.

## Configuration

`Andor.Communications.Service/appsettings.json`:

- `ServiceBus:QueueName` — queue the consumer reads (`request-communication`).
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
