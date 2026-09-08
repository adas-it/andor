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

Migration
[`20260902110428_SeedInitialRulesAndTemplates`](https://github.com/adas-it/andor/blob/main/Src/Communications/Andor.Communications.Infrastructure/Migrations/20260902110428_SeedInitialRulesAndTemplates.cs)
inserts **two rules**, each with **one default English template**. These are applied automatically
on service start-up by `app.ApplyCommunicationMigrationsAsync()`.

### Rules

| `Id` | `Name` | `Type` |
|---|---|---|
| `acb860a5-1af6-4b03-afae-e290dfcac7d4` | `onboarding-verification-code` | `1` (Information) |
| `42b2aa27-82e9-4d20-8e88-afbf25e2c721` | `wellcome` | `1` (Information) |

### Templates

| `Id` | `RuleId` | `Title` | `Lang` | `Partner` | `IsDefault` | `Subject` | `Value` |
|---|---|---|---|---|---|---|---|
| `a382b4e7-db3d-49a8-a15b-c258e86cd6d0` | `acb860a5-…dfcac7d4` (`onboarding-verification-code`) | `wellcome` | `en` | `1` (InHouse) | `true` | `Welcome Email` | `<h1>Hello <name>!</h1><br>Your code is <code>` |
| `d5b1f6c2-8a34-4e79-9c1b-2f7e0a4d6b83` | `42b2aa27-…afbf25e2c721` (`wellcome`) | `wellcome` | `en` | `1` (InHouse) | `true` | `Welcome Email` | `<h1>Welcome <name>!</h1><br>We're glad to have you on board.` |

!!! note "About the first row"
    The `onboarding-verification-code` rule currently carries a template whose `Title` is
    `wellcome`. That is intentional: the Onboarding consumer requests
    `RuleId = acb860a5-…`, `TemplateTitle = "wellcome"`, `ContentLanguage = "en"`, so the lookup
    key matches. The `wellcome` **rule** (`42b2aa27-…`) is configured and ready but is not wired to
    a producer yet — it exists for a future "welcome the user after verification" flow.

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
