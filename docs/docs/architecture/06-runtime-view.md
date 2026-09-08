# 6. Runtime View

Four scenarios cover the mechanisms that matter: a synchronous authenticated write, the actor
load-then-serve lifecycle, the cross-module event flow at signup, and an outbound notification.

## 6.1 Synchronous authenticated command (e.g. create an account category)

```mermaid
sequenceDiagram
    autonumber
    participant C as Client
    participant RP as YARP proxy
    participant Ctrl as Controller : BaseController
    participant Svc as *CommandsService
    participant Mgr as *ManagerActor
    participant Act as *Actor (Ready)
    participant Repo as ICommands*Repository
    participant DB as DbContext + Outbox

    C->>RP: POST /accounts/v1/... (Bearer JWT)
    RP->>Ctrl: forward, strip "/accounts"
    Ctrl->>Ctrl: JWT validated; ICurrentUserService resolves user
    Ctrl->>Svc: MapToCommand(*Input)
    Svc->>Mgr: Ask(command : ICommands<TId>)
    Mgr->>Act: forward (child by aggregate id)
    Act->>Act: apply domain behaviour → DomainResult
    alt domain rules satisfied
        Act->>Repo: persist aggregate
        Repo->>DB: SaveChanges (aggregate + OutboxMessage, one tx)
        Act-->>Svc: ApplicationResult<T>.Success(data)
        Svc-->>Ctrl: ApplicationResult<T>
        Ctrl-->>C: 200/201 DefaultResponse<T> { data, traceId }
    else notification (expected failure)
        Act-->>Svc: ApplicationResult<T> with errors
        Svc-->>Ctrl: ApplicationResult<T>
        Ctrl-->>C: 400/404 DefaultResponse<T> { errors, traceId }
    end
```

Key points: the controller never builds status codes by hand (`BaseController.Result` does the
mapping); expected failures travel as data, not exceptions; the aggregate and its outbox message
commit together.

## 6.2 Actor lifecycle — load then serve

```mermaid
sequenceDiagram
    autonumber
    participant Svc as *CommandsService
    participant Mgr as *ManagerActor
    participant Act as *Actor
    participant Scope as IServiceScope (transient)
    participant Repo as ICommands*Repository

    Svc->>Mgr: Ask(MutateCommand{ Id = X })
    Mgr->>Act: ActorOf(name = X) if absent, then forward
    Note over Act: state = Loading
    Act->>Act: Stash(MutateCommand)
    Act->>Act: Self ! Preload
    Act->>Scope: CreateScope()
    Scope->>Repo: GetById(X)
    Repo-->>Act: aggregate (or null)
    Act->>Act: Become(Ready); Stash.UnstashAll()
    Note over Act: Loading also handles CreateXCommand directly
    Act->>Act: process MutateCommand in Ready
```

One `*Actor` instance per aggregate id means **all commands for that aggregate are serialized** —
no optimistic-concurrency retries, no distributed lock. State is held in memory only after a
successful load; a fresh DI scope per load keeps the `DbContext` short-lived.

## 6.3 Cross-module flow — signup provisions identity + account

```mermaid
sequenceDiagram
    autonumber
    participant V as Visitor
    participant ONB as onboarding-api
    participant OBX as Outbox + Dispatcher
    participant SB as Azure Service Bus
    participant COM as communications-api
    participant USR as users-api
    participant ACC as accounts-api

    V->>ONB: POST /onboarding/start { name, email }
    ONB->>ONB: SignupActor generates 6-digit code
    ONB->>OBX: persist SignupRequest + OutboxMessage(SignupCodeGenerated)
    OBX->>SB: relay SignupCodeGenerated
    SB->>COM: request-communication (SendNotificationInput)
    COM->>V: e-mail with code (SMTP / InHousePartner)

    V->>ONB: POST /onboarding/verify { email, code, password }
    ONB->>ONB: validate code; hash password (PasswordHasher)
    ONB->>OBX: persist verified state + OutboxMessage(SignupVerifiedDomainEvent)
    OBX->>SB: relay SignupVerifiedDomainEvent { userId, passwordHash }
    par fan-out, same event
        SB->>USR: UserVerifiedConsumer → create user identity
    and
        SB->>ACC: UserVerifiedConsumer → create first account
    end
```

Onboarding never learns the e-mail wording, the channel, or that Users and Accounts exist as
separate stores — it just raises events. Consumers must be **idempotent** (at-least-once
delivery). The password is hashed before it ever reaches the domain layer or an event.

## 6.4 Outbound notification — request to delivery

```mermaid
sequenceDiagram
    autonumber
    participant Mod as Any module
    participant SB as Service Bus queue<br/>request-communication
    participant Cons as RequestCommunicationConsumer<br/>(BackgroundService)
    participant Mgr as RuleManagerActor
    participant Act as RuleActor (per RuleId)
    participant P as InHousePartner (SMTP)

    Mod->>SB: SendNotificationInput { RuleId, RecipientEmail, TemplateTitle, ContentLanguage, Values }
    Cons->>Mgr: IRuleCommandsService.SendNotificationAsync(input)
    Mgr->>Act: forward (one child per RuleId ⇒ sequential per rule)
    Act->>Act: load Rule + Templates; pick Template where Title == x && ContentLanguage == y
    alt template found
        Act->>P: render (string.Replace over Values on Subject + Value), send
        P-->>Cons: success
        Cons->>SB: CompleteMessageAsync
    else RuleNotFound (5001)
        Act-->>Cons: errors
        Cons->>SB: AbandonMessageAsync (Service Bus retries, then dead-letters)
    end
```

A REST endpoint (`POST /v1/Communications/notifications`) reaches the same
`RuleManagerActor → RuleActor` path and is meant for operational one-offs; production traffic goes
through the queue so delivery stays decoupled and retryable. See the
[Communications domain page](../domains/communications.md) for `Rule`/`Template` details.

## 6.5 Error & recovery behaviour

| Situation | Behaviour |
|---|---|
| Domain rule violated | `Notification` / `DomainErrorCode` → `ApplicationResult` errors → `400/404` with `TraceId`. No exception, no partial write. |
| Unexpected exception | `GlobalExceptionHandlerMiddleware` → `500` `DefaultResponse` with `TraceId`; details logged, not leaked. |
| Outbox message send fails | Row stays pending; `OutboxDispatcher` retries on its next poll. |
| Consumer processing fails | Message abandoned → Service Bus redelivers, then dead-letters per its policy. |
| Service restart | Actors are re-created lazily and reload from the repository on first command; EF migrations are applied on start. |
