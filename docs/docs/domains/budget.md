# Budget Domain

<details>
  <summary>Diagrama UML - Conta Corrente</summary>

![Conta Corrente](https://kroki.io/plantuml/svg/eNpJzC0KwjAUhu_9icRYH3b9xOs2bUsji6IjQjqAqz2dt6NfR29qlmvmeLyL6BLq3gDk4VylDiRzy9Cc6XX7BhfH8KY3MQxZSE1GCFRqKzKMnZPJN3KH_52P5DFwrTqWHTyTwUYF6B4weEF5xKjOkyS3pSmwDQGxhoK8Ce81GcLqEgGaEZ4un1jyXLZxLC9R1uMAOQUwQs1e11p4WnYXBp1g8A6ieI_5OYGGcf1ufoJK77EDVy4gNl9L6MmhSCVJ16vlL_fBAnu52A)

</details>

## Description

The Budget domain (project family `Andor.Accounts.*`, folder `Src/Budget`) owns **personal
cash management**: shared current accounts, the categories / sub-categories / payment methods
used to classify money, invitations to co-own an account, the **financial movements** (income
and expense entries) and the **CashFlow** monthly-balance projection built from them.

It is the largest write model in the platform. Every mutation goes through the `Account`
aggregate via an Akka.NET actor; reads are served by dedicated `*QueriesService` classes.

| Fact | Value |
|---|---|
| Deployable service | `accounts-api` (Aspire) |
| Public route prefix (reverse proxy) | `/accounts/*` → forwarded to the service with `/accounts` stripped |
| Controller base route | `v{version:apiVersion}/account` (and `v1/account/...`) |
| Actor system | `AndorAccountsSystem` |
| DB schema | `Accounts` (outbox: `AccountsOutbox`), context `AccountsContext` |
| Integration topic | `andor-accounts-events` (all `Account*` domain events) |

## Domain model

### `Account` — aggregate root (`AccountId`)

A shared ledger. Implements `ISoftDeletableEntity` (`IsDeleted`); the creating user is added
as an **Owner** member on creation.

| Field | Notes |
|---|---|
| `Id` (`AccountId`) | `uniqueidentifier`. |
| `Name` | `Name` value object. |
| `Description` | `Description?` value object. Today the REST layer stores the account **name** here (see *Known quirks*). |
| `Currency` | `Currency` entity, resolved from `CurrencyId` at creation; `CurrencyNotFound` if unknown. |
| `IsDeleted` | Soft-delete flag. |
| `LastUpdate` | UTC, bumped on every financial-movement add / edit / remove. |
| `Categories` | `AccountCategory[]` — categories attached to this account (ordered). |
| `SubCategories` | `AccountSubCategory[]` — sub-categories attached to this account (ordered). |
| `PaymentMethods` | `AccountPaymentMethod[]` — payment methods attached to this account (ordered). |
| `Members` | `AccountUser[]` — users with a `PermissionType` on the account. |
| `Invites` | `Invite[]` — pending / answered invitations. |

### Classification entities

`Category`, `SubCategory` and `PaymentMethod` each exist in two forms:

- **Template** (`IsTemplate` — `Owner`/`AccountId` is null): a global, reusable definition.
  Templates are what a fresh account is seeded with.
- **Custom** (owned by one `AccountId`): created ad-hoc for a single account.

| Entity | Key fields | Rules |
|---|---|---|
| `Category` | `Name`, `Description`, `MovementType` (`Type`) | `MovementType` cannot be `Undefined` (6000); name required (6001). |
| `SubCategory` | `Name`, `Description`, `CategoryId`, optional `DefaultPaymentMethodId` | Parent category must belong to the same account; a default payment method must belong to the account **and** share the category's `MovementType`. |
| `PaymentMethod` | `Name`, `Description`, `MovementType` (`Type`) | `MovementType` cannot be `Undefined` (7000); name required (7001). |

### `FinancialMovement` — entity under `Account` (`FinancialMovementId`)

One income or expense line. Implements `ISoftDeletableEntity`.

| Field | Notes |
|---|---|
| `Date` | When the movement occurs. May be in the past (this is a personal ledger, not a bank). |
| `Description` | Optional free text. |
| `SubCategoryId` / `SubCategory` | Classifies the movement. |
| `Type` (`MovementType`) | **Derived** from `SubCategory.Type` — never set directly. |
| `PaymentMethodId` / `PaymentMethod` | Must belong to the account and match `SubCategory.Type`. |
| `Value` | `decimal`, **must be > 0**. |
| `Status` (`MovementStatus`) | `Expected` (forecast) or `Accomplished` (settled). Defaults to `Expected`. |
| `IsDeleted` | Soft-delete flag. |

Validation: `Value > 0` (5000), `Date` at most 5 years in the future (5001),
payment-method type must equal sub-category type (5006).

### `CashFlow` — read-model projection (`CashFlowId`)

A **projection**, not an aggregate: it raises no events and is never written by `Account`.
One row per `account × year × month` (`PeriodKey = year*100 + month`). Maintained solely by
`CashFlowProjectionConsumer`.

| Field | Meaning |
|---|---|
| `FinalBalancePreviousMonth` | Opening balance (previous month's closing balance). |
| `MonthRevenues` | Settled income this month. |
| `ForecastUpcomingRevenues` | `Expected` income this month. |
| `Expenses` | Settled expenses this month. |
| `ForecastExpenses` | `Expected` expenses this month. |
| `AccountBalance` | `RevenuesBalance − Expenses` (settled only). |
| `RevenuesBalance` (computed) | `MonthRevenues + FinalBalancePreviousMonth`. |
| `BalanceForecast` (computed) | `(RevenuesBalance + ForecastUpcomingRevenues) − (Expenses + ForecastExpenses)`. |
| `MonthlyDeficitSurplus` (computed) | `(MonthRevenues + ForecastUpcomingRevenues) − (Expenses + ForecastExpenses)`. |

### Enumerations

| Enum | Values |
|---|---|
| `MovementType` | `0 Undefined`, `1 MoneyDeposit`, `2 MoneySpending` |
| `MovementStatus` | `0 Undefined`, `1 Accomplished`, `2 Expected` |
| `PermissionType` | `0 Undefined`, `1 Viewer`, `2 Editor`, `2 Owner` — see *Known quirks* |

## Permissions

Every mutating method on `Account` takes the acting `userId` and checks membership:

| Capability | Minimum role |
|---|---|
| Read | any member (`Viewer`) |
| Add/create categories, sub-categories, payment methods; add/edit/remove financial movements | `Editor` or `Owner` |
| Link/remove members, invite, respond to invites on behalf of the account, soft-delete the account | `Owner` |

An account must always keep **at least one Owner** (`AccountShouldHaveOneOwner`, 3002).

## Write model (actor flow)

`IAccountCommandsService` → `AccountManagerActor` → one `AccountActor` per `AccountId`
(`Manager → Actor → Stash` pattern). `AccountActor` loads the aggregate once, then serves:

| Command | State | Effect |
|---|---|---|
| `CreateAccountCommand` | `Loading` | Resolves the currency, `Account.NewAsync`, persists, raises `AccountCreatedDomainEvent`. |
| `SeedAccountDefaultsCommand` | `Ready` | Attaches all current template categories → payment methods → sub-categories (in that order). |
| `AddFinancialMovementCommand` | `Ready` | Builds a `FinancialMovement` from account-local sub-category/payment-method instances, validates, persists, raises `AccountFinancialMovementAddedDomainEvent`. |
| `EditFinancialMovementCommand` | `Ready` | In-place edit when month **and** type are unchanged (`...EditedDomainEvent`); otherwise remove + re-add (`Removed` + `Added`) so the CashFlow buckets stay correct. |
| `DeleteFinancialMovementCommand` | `Ready` | Soft-deletes the movement, raises `AccountFinancialMovementRemovedDomainEvent`. |

## Endpoints

All under `v1/account`. JWT required (global fallback policy — no `[AllowAnonymous]` here).

### Accounts

| Method | Route | Body / Query | Description |
|---|---|---|---|
| `POST` | `/v1/account` | `AccountInput` | Create an account; caller becomes Owner. |
| `POST` | `/v1/account/{accountId}/seed` | — | Attach the current default templates to the account. |
| `GET` | `/v1/account` | `page`, `per_page`, `search`, `sort`, `dir` | Paginated list of the caller's accounts (`ListAccountOutput`). |
| `GET` | `/v1/account/{id}` | — | Single account (`AccountOutput`), `404` if not found. |
| `GET` | `/v1/account/{accountId}/cash-flow` | `year`, `month` (default: current) | `CashFlowOutput` for the period. |
| `GET` | `/v1/account/{accountId}/financial-summary` | `year`, `month` | `List<FinancialSummariesOutput>`. |
| `GET` | `/v1/account/{accountId}/category-summary` | `year`, `month` | `List<CategorySummariesOutput>`. |

### Financial movements

| Method | Route | Body | Description |
|---|---|---|---|
| `POST` | `/v1/account/{accountId}/financial-movement` | `RegisterFinancialMovementInput` | Register a movement (`FinancialMovementOutput`). |
| `PUT` | `/v1/account/{accountId}/financial-movement/{id}` | `ModifyFinancialMovementInput` | Edit a movement. |
| `DELETE` | `/v1/account/{accountId}/financial-movement/{id}` | — | Soft-delete a movement (`204`). |
| `GET` | `/v1/account/{accountId}/financial-movement/{financialMovementId}` | — | Single movement. |
| `GET` | `/v1/account/{accountId}/financial-movement` | `page`, `per_page`, `search`, `sort`, `dir`, `year`, `month` | Paginated list (`ListFinancialMovementsOutput`). |

### Classification (read-only today)

| Method | Route | Query | Description |
|---|---|---|---|
| `GET` | `/v1/account/{accountId}/category/{categoryId}` | — | Single category. |
| `GET` | `/v1/account/{accountId}/category` | `page`, `per_page`, `search`, `sort`, `dir`, `type` | Paginated categories. |
| `GET` | `/v1/account/{accountId}/sub-category/{subCategoryId}` | — | Single sub-category. |
| `GET` | `/v1/account/{accountId}/sub-category` | `page`, `per_page`, `search`, `sort`, `dir`, `category` | Paginated sub-categories. |
| `GET` | `/v1/account/{accountId}/payment-method/{paymentMethodId}` | — | Single payment method. |
| `GET` | `/v1/account/{accountId}/payment-method` | `page`, `per_page`, `search`, `sort`, `dir`, `type` | Paginated payment methods. |

## Contracts

```csharp
// requests
record AccountInput(string Name, string CurrencyId);

record RegisterFinancialMovementInput {
    DateTime Date; string? Description; decimal Value;
    Guid SubCategoryId; int StatusId; Guid PaymentMethodId; Guid AccountId;
}
// ModifyFinancialMovementInput has the same shape.

// responses
record AccountOutput {
    string Id; string Name; string Description; bool Deleted;
    DateTime? FirstMovement; DateTime? LastMovement;
    List<ParticipantOutput> Participants;   // Id, FullName, Avatar, AvatarThumbnail, Status(Key,Name)
}

record FinancialMovementOutput {
    Guid Id; DateTime Date; string? Description; decimal Value;
    SubCategoryOutput SubCategory; MovementTypeOutput Type;
    FinancialMovementStatusOutput Status; PaymentMethodOutput PaymentMethod;
}

record CashFlowOutput {
    Guid Id; int Year; int Month;
    decimal MonthRevenues, FinalBalancePreviousMonth, ForecastUpcomingRevenues, RevenuesBalance,
            Expenses, AccountBalance, ForecastExpenses, BalanceForecast, MonthlyDeficitSurplus;
}
```

`StatusId` maps to `MovementStatus` (`1 = Accomplished`, `2 = Expected`).

## Business rules

- **Seeding is explicit and idempotent-by-intent.** A new account starts empty; templates are
  attached by `SeedAccountDefaultsCommand` (called automatically by `AccountCreatedConsumer`,
  or manually via `POST /{accountId}/seed`). Order matters: categories and payment methods
  must exist on the account before sub-categories, because `AddTemplateSubCategory` checks that
  the sub-category's category *and* its default payment method already belong to the account.
- **Everything a movement points at must belong to the same account** — sub-category, its
  parent category, and the payment method. The payment method's `MovementType` must equal the
  sub-category's.
- **Movement value must be strictly positive**; direction (income vs expense) comes from
  `MovementType`, not the sign.
- **Edits that change month or movement type are not in-place.** The actor turns them into a
  remove + add so the CashFlow projection (bucketed by `account × month × type`) can move the
  value between rows. Same-month, same-type edits raise `AccountFinancialMovementEditedDomainEvent`
  carrying both the previous and new value/status.
- **CashFlow cascades forward.** Because a movement can land in a past month, applying a delta
  re-anchors every later month that already has a row for the account.
- **Last Owner cannot be removed**, and only Owners manage membership and deletion.

## Domain error codes

### `Account` (3000–3699)

| Code | Name |
|---|---|
| 3001 | `AccountErrorOnDelete` |
| 3002 | `AccountShouldHaveOneOwner` |
| 3003 | `CurrencyNotFound` |
| 3100–3105 | `CategoryCannotBeNull`, `CategoryMustBeTemplate`, `CategoryAlreadyAdded`, `CannotAddDeletedCategory`, `UserNotMember`, `InsufficientPermissions` |
| 3200–3207 | `SubCategoryCannotBeNull`, `SubCategoryMustBeTemplate`, `SubCategoryAlreadyAdded`, `SubCategoryCategoryNotInAccount`, `PaymentMethodShouldBeSameTypeAsCategory`, `SubCategoryPaymentMethodNotInAccount`, `SubCategoryCategoryNotFound`, `SubCategoryDefaultPaymentMethodNotFound` |
| 3300–3303 | `PaymentMethodCannotBeNull`, `PaymentMethodMustBeTemplate`, `PaymentMethodAlreadyAdded`, `CannotAddDeletedPaymentMethod` |
| 3400–3402 | `UserCannotBeNull`, `UserAlreadyMember`, `OnlyOwnerCanLinkMembers` |
| 3500–3505 | `InviteCannotBeNull`, `InviteAlreadyExists`, `OnlyOwnerCanInviteMembers`, `CannotInviteExistingMember`, `InviteNotFound`, `UserNotInvited` |
| 3600–3607 | `FinancialMovementCannotBeNull`, `FinancialMovementSubCategoryNotInAccount`, `FinancialMovementPaymentMethodNotInAccount`, `FinancialMovementCategoryNotInAccount`, `FinancialMovementPaymentMethodTypeMismatch`, `FinancialMovementSubCategoryNotFound`, `FinancialMovementPaymentMethodNotFound`, `FinancialMovementNotFound` |

### `Invite` (4001–4006)

| Code | Name |
|---|---|
| 4001 | `InviteNotActive` |
| 4002 | `InviteAlreadyAccepted` |
| 4003 | `CannotRejectAcceptedInvite` |
| 4004 | `InviteAlreadyLinkedToUser` |
| 4005 | `InviteNotLinkedToUser` |
| 4006 | `InviteMustHaveEmailOrUserId` |

### `FinancialMovement` validation (5000–5006)

| Code | Name |
|---|---|
| 5000 | `ValueMustBeGreaterThanZero` |
| 5001 | `DateCannotBeTooFarInFuture` |
| 5002 | `FinancialMovementCannotBeNull` |
| 5003 | `SubCategoryNotInAccount` |
| 5004 | `PaymentMethodNotInAccount` |
| 5005 | `CategoryNotInAccount` |
| 5006 | `PaymentMethodTypeMismatch` |

### `Category` (6000–6001) · `PaymentMethod` (7000–7001)

| Code | Name |
|---|---|
| 6000 | `Category.MovementTypeCannotBeUndefined` |
| 6001 | `Category.NameCannotBeNull` |
| 7000 | `PaymentMethod.MovementTypeCannotBeUndefined` |
| 7001 | `PaymentMethod.NameCannotBeNull` |

### Application layer (`AccountErrorCodes`, 14000–14002)

| Code | Name | HTTP |
|---|---|---|
| 14000 | `NotFound` | `404` |
| 14001 | `AccountValidation` | `400` |
| 14002 | `CurrencyNotFound` | `400` |

## Domain events

All published to topic **`andor-accounts-events`** with the event type as the message subject.

| Event | Raised when |
|---|---|
| `AccountCreatedDomainEvent` | Account created. |
| `AccountDeletedDomainEvent` | Account soft-deleted. |
| `AccountCategoryAddedDomainEvent` | Template or custom category attached. |
| `AccountSubCategoryAddedDomainEvent` | Template or custom sub-category attached. |
| `AccountPaymentMethodAddedDomainEvent` | Template or custom payment method attached. |
| `AccountMemberAddedDomainEvent` / `AccountMemberRemovedDomainEvent` | Membership change. |
| `AccountMemberInvitedDomainEvent` | Invitation created (by user id or email). |
| `AccountMemberInviteAcceptedDomainEvent` / `AccountMemberInviteDeclinedDomainEvent` | Invite answered. |
| `AccountInviteUserLinkedDomainEvent` | A pending e-mail invite got linked to a new user id. |
| `AccountFinancialMovementAddedDomainEvent` | Movement added — carries `Value`, `Status`, `Type`, `Date`, ids. |
| `AccountFinancialMovementRemovedDomainEvent` | Movement removed — carries `Value`, `Status`, `Type`, `Date`. |
| `AccountFinancialMovementEditedDomainEvent` | In-place edit — carries previous **and** new `Value`/`Status`. |

## Integration (consumers hosted by `accounts-api`)

| Consumer | Listens to | Action |
|---|---|---|
| `UserVerifiedConsumer` | `user-verified-events` (`SignupVerifiedDomainEvent` from Onboarding) | Auto-creates a personal `Account` (`"Conta de {name}"`, currency `BRL`) for the newly verified user. |
| `AccountCreatedConsumer` | `andor-accounts-events` (`AccountCreatedDomainEvent`) | Runs `SeedAccountDefaultsCommand` to attach the default templates. |
| `CashFlowProjectionConsumer` | `andor-accounts-events` (`...Added` / `...Removed` / `...Edited`) | Maintains the monthly `CashFlow` rows: apply / reverse / reverse-then-apply, then cascade the balance forward. Pushes each updated row over the `/ws` WebSocket. |

## Persistence

- Schema `Accounts`, one table per entity (`Account`, `AccountUser`, `AccountCategory`,
  `AccountSubCategory`, `AccountPaymentMethod`, `Category`, `SubCategory`, `PaymentMethod`,
  `Invite`, `FinancialMovement`, `CashFlow`, `Currency`).
- Transactional outbox in schema `AccountsOutbox`, relayed by the shared `OutboxDispatcher`.
- Migrations: `Src/Budget/Andor.Accounts.Infrastructure/Migrations` (context `AccountsContext`).

## Known quirks / gaps

- **`AccountController.CreateAsync` stores the account name as its description**
  (`new Description(input.Name)`), so `AccountOutput.Description` echoes the name until a real
  description field is added to `AccountInput`.

## Custom categories, sub-categories and payment methods

`Account.CreateCustomCategory` / `CreateCustomSubCategory` / `CreateCustomPaymentMethod` existed
on the aggregate but had no HTTP surface until now — only template attachment
(`AddTemplateCategory` etc., via `SeedAccountDefaultsCommand`) was wired. Each now has a `POST`:

| Verb | Route | Action |
|---|---|---|
| `POST` | `v1/account/{accountId}/category` | `CreateCustomCategoryCommand`, from `CreateCategoryInput` (`Name`, `Description`, `TypeId`). |
| `POST` | `v1/account/{accountId}/sub-category` | `CreateCustomSubCategoryCommand`, from `CreateSubCategoryInput` (`Name`, `Description`, `CategoryId`, optional `DefaultPaymentMethodId`). The `AccountActor` resolves the `Category`/`PaymentMethod` domain objects off the already-loaded `_account.Categories`/`PaymentMethods` before calling the domain method, failing fast with `SubCategoryCategoryNotFound` / `SubCategoryDefaultPaymentMethodNotFound` (3206/3207) if either id doesn't belong to the account. |
| `POST` | `v1/account/{accountId}/payment-method` | `CreateCustomPaymentMethodCommand`, from `CreatePaymentMethodInput` (`Name`, `Description`, `TypeId`). |

**Persistence bug found and fixed while wiring this up**: `CommandsAccountRepository.PersistAsync`'s
child reconciliation only ever marked the join row (`AccountCategory`/`AccountSubCategory`/`AccountPaymentMethod`)
as `Added` — never the master `Category`/`SubCategory`/`PaymentMethod` row itself. That was
invisible as long as the only caller was template attachment (the master row already exists,
seeded separately); a *custom* create needs the master row inserted too, or the join row ends up
pointing at nothing and the category silently disappears from the next `Include`-based read.
Fixed by reconciling the master entities the same way, scoped to the ids the account now
references (see the three `ReconcileChildStatesAsync` calls added after each join-table one).

## Invites

`InvitesController` (`v1/account/{accountId}/invites`) exposes the invite lifecycle that already
existed on the `Account` aggregate:

| Verb | Route | Action |
|---|---|---|
| `POST` | `.../invites` | Creates an invite, by e-mail (`InviteMemberByEmailCommand`) or by an existing `UserId` (`InviteMemberByUserCommand`) depending on which field of `InviteInput` is set. |
| `GET` | `.../invites` | Lists the account's invites (`IAccountQueriesService.GetInvitesAsync`). |
| `POST` | `.../invites/{inviteId}/answer` | Accepts or rejects (`AnswerInviteInput.Accept`) via `AnswerInviteCommand`. |

`Account.RespondInvite` now adds the invitee as a member synchronously (with the invite's own
`Permission`) instead of requiring a separate `LinkMember` call — there is no domain-event handler
for this; see `Account.AddMemberRecord`, shared with `LinkMember`.

`PermissionType.Editor`/`Owner` no longer share a key (`Owner` is `3`); see migration
`FixPermissionTypeOwnerKeyCollision` for the one-time data fix this required. `InviteId` also
gained a `ToString()` override (it was missing, unlike `AccountId`/`UserId`), since the
`{inviteId:guid}` route depends on it serializing as a plain GUID.

## Next steps

- Wire a Communications producer for the already-seeded `account-invite` rule/template (see
  `Andor.Communications.Infrastructure`'s `SeedRealTemplates` migration) so creating an invite by
  e-mail actually sends one.
- Bridge Onboarding/signup with pending e-mail invites: today, verifying a new signup never checks
  for a pending `Invite` for that e-mail and calls `LinkUserToInvite` automatically.
- Document `FinancialSummariesOutput` / `CategorySummariesOutput` and the `/ws` protocol.
