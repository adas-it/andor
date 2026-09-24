# Backend pendings — Kenobi frontend ↔ Andor backend

This document lists backend work that the frontend (`kenobi`) already expects or will expect,
so that when each item is implemented on `andor` matching the contract below, the frontend
should work without further changes on its side.

It was written while implementing the "Categoria e método de pagamento" design spec
(`Categoria e método de pagamento/design_handoff_settings/`) on the frontend only — no backend
code was touched in that pass. Everything here is a suggestion to keep the two sides aligned;
adjust freely, just keep the frontend slices in `kenobi/src/store/slices/*.ts` in sync if a
route/shape changes.

Legend: 🔴 blocks a shipped frontend screen today · 🟡 nice-to-have / edge case · 🔵 future work, not started anywhere yet.

---

## 1. Category — update & delete 🔴

Today `Andor.Accounts.RestApi/AccountCategoriesController` only has Create + Read
(`POST`/`GET v1/account/{accountId}/category`). The new "Configurations → Categories" screen
(`kenobi/src/views/pages/configurations/CategoriesAndPaymentMethods.tsx`) and the
"Edit Category" modal (`.../components/CreateCategoryDialog.tsx`) already call:

```
PUT    v1/account/{accountId:guid}/category/{categoryId:guid}
DELETE v1/account/{accountId:guid}/category/{categoryId:guid}
```

Suggested request body for `PUT` (mirrors `CreateCategoryInput` plus the new fields in §3):

```csharp
public record UpdateCategoryInput
{
    public string Name { get; init; }
    public string Description { get; init; }
    public int TypeId { get; init; }
    public string? Avatar { get; init; }   // see §3
    public string? Color { get; init; }    // see §3
}
```

`DELETE` should be a soft delete (`Category.IsDeleted`, matching the existing domain flag) and
should fail with a clear error code if the category still has sub-categories or linked
financial movements — the frontend shows the server error via the snackbar but doesn't have a
dedicated "category has movements" confirmation flow yet, so a descriptive `AccountErrorCode`
is enough for now.

Frontend call sites: `kenobi/src/store/slices/categoriesMovement.ts`
(`useUpdateAccountCategoryMutation`, `useDeleteAccountCategoryMutation`).

## 2. SubCategory — update & delete 🔴

Same gap as above, mirrored for `AccountSubCategoriesController`:

```
PUT    v1/account/{accountId:guid}/sub-category/{subCategoryId:guid}
DELETE v1/account/{accountId:guid}/sub-category/{subCategoryId:guid}
```

`UpdateSubCategoryInput` can mirror the existing `CreateSubCategoryInput`
(`Name`, `Description`, `CategoryId`, `DefaultPaymentMethodId`). Delete should block (or ask to
confirm, per the design spec §1b "Interactions & Behavior") when the sub-category already has
linked movements — same pattern as category delete.

Frontend call sites: `kenobi/src/store/slices/subCategoriesMovement.ts`
(`useUpdateAccountSubCategoryMutation`, `useDeleteAccountSubCategoryMutation`).

Note: **creating** a sub-category already works end-to-end today (`POST .../sub-category`), so
the "New Category" modal (spec §1b) can create a category + several sub-categories in one flow
right now by issuing one `POST .../category` followed by N `POST .../sub-category` calls — no
new "combined" endpoint is needed for create, only for edit/delete.

## 3. Category — Avatar/Color fields 🔴

The design spec's "New Category" modal (`02-criar-categoria.md`) has an "Icon & Color" picker
that has no backend field to persist into. Today's `Category` domain entity and `CategoryOutput`
DTO have no `Avatar`/`Color` (the frontend `Category` type carries an `avatar: string` field
that was already unused dead weight before this change — now it's actually wired to the new UI).

Suggested: add two nullable string columns to `Category` (`Avatar`, `Color`), returned in
`CategoryOutput` and accepted in `CreateCategoryInput`/`UpdateCategoryInput`. The frontend picks
`Avatar` from a small fixed key set (`local_grocery_store`, `account_balance`, `maps_home_work`,
`commute`, `school`, `important_devices`, `medication_liquid`, `groups`, `skateboarding`,
`savings` — see `kenobi/src/views/pages/configurations/components/CategoryIconPicker.tsx`) and
`Color` as a hex string (e.g. `#ffab91`). No validation needed beyond "is one of the known keys"
if you want to be strict, otherwise a free string is fine since the frontend already falls back
to a default icon/color when the value is unknown.

## 4. PaymentMethod — kind + credit card fields 🔴🟡

This is the biggest gap and has a **naming clash** worth resolving deliberately, not blindly:

- Today's `PaymentMethod.Type` (`MovementType`: Money Deposit / Money Spending) is unrelated to
  the new "Type" field in the design spec (`03-metodo-pagamento.md`), which is really a
  **payment-method kind**: Cash / Debit / Credit / Transfer / Automatic Debit / Withheld at
  Source.
- The new modal (`kenobi/src/views/pages/configurations/components/CreatePaymentMethodDialog.tsx`)
  does **not** show a Money-Deposit/Spending selector at all — it only asks for the new "kind".
  The frontend currently sends a hardcoded `typeId: 2` (Money Spending) just to satisfy today's
  required field, which is almost certainly wrong for e.g. a "Salary transfer" payment method
  used on deposits. **Please double check with product/design whether `PaymentMethod.Type`
  (movement type) should be dropped entirely now that sub-categories already carry their own
  movement type, or whether it should stay and get its own UI control.**

Suggested `PaymentMethodKind` enum (frontend numbering, adjust freely, just keep both sides in
sync):

```
1 = Cash, 2 = Debit, 3 = Credit, 4 = Transfer, 5 = AutomaticDebit, 6 = WithheldAtSource
```

Suggested fields to add to `PaymentMethod` domain + `PaymentMethodOutput` /
`CreatePaymentMethodInput` (new `UpdatePaymentMethodInput` too, see below):

```csharp
public int Kind { get; init; }                 // PaymentMethodKind
public int? ClosingDay { get; init; }           // 1–31, Credit only
public int? DueDay { get; init; }               // 1–31, Credit only
public decimal? Limit { get; init; }            // >= 0, Credit only, optional
public bool? RolloverAfterClosingDay { get; init; } // Credit only — "purchases after closing go to next month's bill"
```

Validation per the design spec (`03-metodo-pagamento.md` → "Validação / erros"): `ClosingDay`/
`DueDay` required and in `1..31` only when `Kind == Credit`; `Limit` optional but must be `>= 0`
when present; `Name` required and unique among the account's payment methods.

## 5. PaymentMethod — update & delete 🔴

Same Create-only gap as Category/SubCategory:

```
PUT    v1/account/{accountId:guid}/payment-method/{paymentMethodId:guid}
DELETE v1/account/{accountId:guid}/payment-method/{paymentMethodId:guid}
```

Delete should block (soft-delete guard) when the payment method is still referenced by a
sub-category's `DefaultPaymentMethod` or by financial movements.

Frontend call sites: `kenobi/src/store/slices/paymentMethodsMovement.ts`
(`useUpdateAccountPaymentMethodMutation`, `useDeleteAccountPaymentMethodMutation`,
`useCreateAccountPaymentMethodMutation` already sends the new `kind`/credit-card fields from §4,
today's backend will just ignore/reject the extra fields until it's extended).

---

## 6. Users — profile update / change password / deactivate 🔴

Unrelated to this feature, but a pre-existing gap the frontend already assumes is fixed:
`kenobi/src/store/slices/users.ts` calls these, and only `GET`/`POST v1/users` exist today in
`Andor.Users.RestApi/UsersController`:

```
PUT    v1/users              — update the current user's profile (name, phone, birthday, avatar…)
PATCH  v1/users/change-password
POST   v1/users/deactivate-account
```

`PUT v1/users` should accept whatever `UserFullFill` sends today
(`kenobi/src/types/user.ts`) — at minimum `firstName`, `lastName`, `email`, `phone`, `birthday`,
`avatar`, `avatarThumb`. See §7 for how `avatar`/`avatarThumb` should actually get populated.

## 7. Avatar upload 🔵

The profile page (`kenobi/src/views/application/users/profile/Profile.tsx`) has an "Upload
Avatar" button today with **no click handler** — it's a dead placeholder. There is no upload
endpoint anywhere in the backend (`grep` for `avatar|upload|IFormFile` across `Src` only turns
up the unrelated `ParticipantOutput.Avatar` string field on account participants, itself
unpopulated).

Suggested contract:

```
POST v1/users/avatar   — multipart/form-data, field name "file", image only, size-limited
                          → { "avatarUrl": "https://...", "avatarThumbUrl": "https://..." }
```

This needs a storage decision (Azure Blob Storage is the natural choice given the rest of the
stack is already on Azure App Service) plus basic validation (content-type allow-list, max size,
maybe a thumbnail generation step for `avatarThumbUrl`). Once this exists, wire the frontend's
"Upload Avatar" button to open a file picker, call this endpoint, then call `PUT v1/users` (§6)
with the returned URLs — that part is pure frontend work once the endpoint exists, not included
in this pass.

## 8. Chat 🔵 — planned for later, SignalR

Confirmed: **no chat feature and no SignalR package anywhere in the solution today**
(`Directory.Packages.props` has no `Microsoft.AspNetCore.SignalR*` reference, and no
chat/message domain exists in `Src/Communications` or elsewhere).

Per direction from the project owner, this is explicitly **not being built now** — implementation
is planned for later using **SignalR** (not the raw WebSocket endpoint described in §9). No
endpoint contract is proposed here yet; when it's picked up, a reasonable starting shape is:

- A `ChatHub : Hub` in a new (or existing `Andor.Communications`) service, mounted at e.g.
  `/hubs/chat`, authenticated the same way as the REST API (OpenIddict bearer token via
  `.AddAuthentication()` on the hub connection).
- Message persistence (sender, account/conversation id, body, timestamp, read state) needs a
  home — `Andor.Communications.Domain` is the natural fit given the existing bounded-context
  layout, but this should be scoped properly when the feature is actually picked up rather than
  guessed at here.
- Frontend has no chat UI, Redux slice, or `@microsoft/signalr` dependency yet either — all of
  that is greenfield work for whenever this is scheduled.

## 9. Existing raw WebSocket endpoint — clarify its role 🟡

`Andor.Accounts.Service` already wires a raw ASP.NET Core WebSocket endpoint
(`app.UseWebSockets()` + `/ws?id={clientId}&sessionId={sessionId}` in `Program.cs`, backed by
`WebSockets/WebSocketMessages.cs` and `WebSockets/Class.cs`). It looks like a push mechanism,
but the frontend's `WebSocket.tsx` component doesn't actually open a `WebSocket` connection —
it's an RTK Query polling wrapper despite the name. Worth checking who (if anyone) currently
calls `IWebSocketMessage.SendAsync` server-side, and deciding whether real-time notifications
(cash-flow refresh, future chat) should consolidate on SignalR instead of carrying two
real-time stacks.

---

## 10. Invite members modal (Etapa 5 of the design handoff) 🔴🟡

Implemented on the frontend this pass: `kenobi/src/ui-component/cards/InviteMembersDialog.tsx`
(replaces the old empty `ShareDialog` stub), triggered by the new "Invite" button on
`AccountSelectorCard.tsx`. **The floating chat bubble described in the same spec file
(`05-invite-members.md`) was intentionally NOT built — chat is deferred to a later pass using
SignalR, per direction from the project owner. See §8.**

What's already wired to real endpoints:
- Send invite: `POST v1/account/{accountId}/invites` (`InviteInput{Email, PermissionKey}`) — works today.
- List invites / derive "pending": `GET v1/account/{accountId}/invites`, filtered client-side to
  `IsActive && !IsAccepted` — works today.
- Members tab reads `GET v1/account/{id}` → `AccountOutput.Participants` — works today, but see
  the gap below, the data is mostly empty.

What's missing, needed to make the modal fully functional. The frontend now calls every one of
these mutations already (see `kenobi/src/store/slices/invites.ts`) and shows a friendly
"not available yet" toast/tooltip on failure — nothing further to change on the frontend once each
of these lands, only the exact request/response shape should be double-checked against what's
below:

- **Cancel a pending invite** 🔴 — no endpoint. `Invite.Deactivate()` already exists on the domain
  entity (`Src/Budget/Andor.Accounts.Domain/Invites/Invite.cs`), nothing calls it. Suggested:
  `DELETE v1/account/{accountId}/invites/{inviteId}` (Owner-only), calling `Account.CancelInvite`
  (new domain method wrapping `Invite.Deactivate()`, mirroring how `RemoveMember` is guarded).
  Frontend calls this from `useCancelInviteMutation` — 404s today.
- **Member name/email/avatar/role on `AccountOutput`** 🔴 — `ParticipantOutput` already has the
  right shape for `FullName`/`Avatar`/`AvatarThumbnail`/`Status` but
  `AccountMapperExtensions.ToAccountOutput` only ever sets `Id`. Also needs: an `Email` field (not
  on `ParticipantOutput` at all today), and a `PermissionKey`/`PermissionName` pair (mirroring
  `InviteOutput`). Today the frontend renders each member row with just an initial-letter avatar,
  no email line, and a role `<select>` stuck on a disabled "Unknown role" placeholder because
  there's nothing to preselect — once `PermissionKey` is populated the same `<select>` will show
  and drive the real role immediately, no frontend change needed.
- **Remove a member** 🔴 — no endpoint. `Account.RemoveMember(User, Guid)` is fully implemented
  and already blocks removing the last Owner. Suggested:
  `DELETE v1/account/{accountId}/members/{userId}` (Owner-only). Frontend calls this from
  `useRemoveMemberMutation` on every row where `permissionKey !== Owner`; because `permissionKey`
  isn't populated yet (see above), the button is conservatively kept **disabled** for every member
  today rather than guessing who the Owner is — it activates automatically once the role field
  exists.
- **Change a member's role** 🟡 — no endpoint **and no domain method** (`Account` has no
  `ChangeMemberPermission`/`UpdateMemberRole`). Frontend's Members tab already has a role
  `<select>` wired to `useUpdateMemberRoleMutation` (`PUT v1/account/{accountId}/members/{userId}`,
  body `{ permissionKey }` — suggested shape, adjust freely) — it 404s today, needs new domain
  logic before it's worth turning on server-side.
- **Invite link (shareable, no-email invite)** 🔵 — the design spec (`05-invite-members.md`) has a
  "copy invite link" affordance; there is **no concept of a shareable/tokenized invite** in the
  domain at all today (only "invite this exact email" or "invite this exact existing user"). The
  frontend now renders the box from the design (dashed border, link icon, "Copy link" button) for
  visual fidelity, but it's permanently disabled with a tooltip explaining why — no fake link is
  generated or copied. Turning it on for real needs a new invite-by-token flow (generate a link
  with an embedded, single-use or expiring token; a `GET`/`POST` to redeem it) designed from
  scratch.
- **Invite `Message` field** 🟡 — the modal's optional "personal message" textarea is present in
  the UI for fidelity with the design but isn't sent anywhere — `InviteInput` has no `Message`
  field, and there's no invite email template in `Andor.Communications` documented as consuming
  one. If invite emails should carry a custom message, add `Message` to `InviteInput` and thread
  it through to whatever sends the invite email.

## 11. Account settings modal (Etapa 4) ✅ frontend done, backend thin — Profile revamp (Etapa 6) still not started 🔵

Progress on the 6-stage design handoff (`Categoria e método de pagamento/design_handoff_settings/README.md`):

- ✅ 1d Configurations list, 1b Create category, 1c New payment method (earlier pass)
- ✅ 3a Invite members modal (earlier pass, chat bubble excluded)
- ✅ **3c Account settings** (`04-account-settings.md`, this pass) —
  `kenobi/src/ui-component/cards/AccountSettingsDialog.tsx`, triggered by the new "Manage
  accounts" button on `AccountSelectorCard.tsx` (opening it closes the Invite modal and vice
  versa, per spec). Left rail lists the user's accounts (`accountsList` from `AccountContext`,
  already available — no extra request needed); clicking one loads its detail
  (`GET v1/account/{id}`) into the right panel: name/currency fields, an inline "Invite member"
  form, the same disabled invite-link placeholder as §10, a Members list with an editable role
  `<select>` and a remove button, Pending invites, and a "Danger zone" card with **both** "Delete
  account" and "Leave account" as separate buttons (each behind a type-the-account-name inline
  confirmation step) — see the note below on why both are shown instead of switching on the
  user's role.

  Backend readiness is thin, and the frontend already calls every one of these — same "will just
  work once implemented" pattern as §1–§10:
  - **List accounts** ✅ works today: `GET v1/account` (used for the rail via `AccountContext`,
    which already includes `Participants` per account — that's how the rail shows a member count
    with zero extra requests).
  - **Get one account** ✅ works today: `GET v1/account/{id}` (right panel detail).
  - **Update account name/currency** 🔴 — no endpoint; `AccountInput` is create-only and there's
    no domain method to change `Currency` post-creation. Frontend calls
    `PUT v1/account/{id}` with body `{ name, currencyId }` (`useUpdateAccountMutation`,
    `kenobi/src/store/slices/account.ts`) — **`currencyId` is a guess**, assumed to stay an ISO
    code string (`"BRL"`/`"USD"`/`"EUR"`, hardcoded list on the frontend for now, matching what
    `AccountInput.CurrencyId` looks like it expects at creation) — please confirm/correct the
    shape once this is actually implemented, whether it should reference `Currency.Id` (a real
    id) instead.
  - **Delete account** 🔴 — no endpoint; `Account.SoftDelete(Guid)` is fully implemented
    (Owner-only). Frontend calls `DELETE v1/account/{id}` (`useDeleteAccountMutation`).
  - **Leave account** 🔴 — no endpoint **and no domain method at all** (self-removal by a
    non-Owner is a different operation from `RemoveMember`, which is Owner-removing-someone-else).
    Frontend calls `POST v1/account/{id}/leave` (`useLeaveAccountMutation`) — needs new domain
    logic designed from scratch (and should presumably reuse the "block removing the last Owner"
    guard `RemoveMember` already has, for the case an Owner tries to leave instead of transferring
    ownership or deleting).
  - **Currency on `AccountOutput`** 🔴 — `Account.Currency` exists on the domain aggregate but
    isn't mapped to `AccountOutput`/`ListAccountOutput` at all, so the modal's currency field has
    nothing to preselect today (defaults to `"BRL"`).
  - Member role `<select>`, remove button, invite/cancel-invite, and the invite-link placeholder
    reuse the exact same (already documented) gaps as §10 — nothing new there, just reused in a
    second surface.

  **On "Delete account" vs. "Leave account"**: the design spec says the card should show only one
  of the two, switching on whether the current user is that account's Owner. There is currently no
  way for the frontend to know that (no `PermissionKey` for the current user on any account —
  same root gap as §10's member-role note), so rather than guess wrong, both destructive actions
  are shown side by side and left for the backend to authorize/reject appropriately (Owner-only for
  delete, non-Owner-only — or Owner-excluded — for leave). Once per-account role is exposed
  (probably via the same `GET v1/account` response, e.g. a `CurrentUserPermissionKey` field), swap
  this back to a single conditional button — no other change needed.

- ⬜ **3b Profile page revamp** (`06-profile.md`) — Profile/Security tabs, a "Sessions" list with
  per-session "Revoke", and a link out to the Account settings modal instead of the current
  Profile page's inline shared-account UI. Needs §6 (`PUT /v1/users`, change-password) plus a new
  sessions-list concept (list + revoke a specific refresh token/session) that doesn't exist
  anywhere in `Andor.Users`/`Andor.Authentication.Jwt` today. Not started on either side yet.

## 12. Template vs. custom Category/SubCategory/PaymentMethod — `IsTemplate` not exposed on any output 🔴

Confirmed business rule (per the project owner): **template** categories/sub-categories/payment
methods — the seeded, shared ones with `Owner == null` (`Category.IsTemplate`,
`Tests/Budget/Andor.Accounts.Domain.Tests` already has `AccountAddTemplateCategoryTests` covering
this concept) — must **not** be editable by an account's users. Only **custom** ones (created by
that account, `Owner != null`) can be edited. Templates can still be **removed from the account**
(distinct from editing them), but not renamed/re-typed/re-iconed/etc.

**None of `CategoryOutput`, `SubCategoryOutput`, `PaymentMethodOutput` expose this today** — there
is no `IsTemplate` (or `Owner`) field on any of the three DTOs, so the frontend has no way to tell
template rows from custom ones.

What the frontend already does about it (`kenobi/src/views/pages/configurations/CategoriesAndPaymentMethods.tsx`):
- `Category`, `SubCategory`, `PaymentMethod` (`types/app/financial-movement.ts`) all got an
  optional `isTemplate?: boolean` field.
- In all three list tabs, a row with `isTemplate === true` hides the **Edit** icon entirely, shows
  a small "Template" badge next to the name, and the **Delete** icon's tooltip switches to "Remove
  from account" instead of "Delete" (same click handler/endpoint for now — see below).
- **This does nothing yet**: since the field doesn't exist in any API response, every row is
  currently treated as `isTemplate: false` (falls back to "editable"), so today's behavior is
  unchanged. The UI will start enforcing the rule automatically the moment the field is added —
  no further frontend work needed for the read side.

What's needed on the backend:
- Add `IsTemplate` (bool) to `CategoryOutput`, `SubCategoryOutput`, `PaymentMethodOutput` — derived
  the same way the domain already does it (`Owner == null`), mapped in each corresponding
  `*MapperExtensions`.
- **Semantic decision needed**, not just plumbing: today's `DELETE`/soft-delete endpoints (§1, §2,
  §5 above, once built) operate on the entity itself. For a template, "remove from account" is
  presumably a different operation — e.g. unlinking/hiding that template from this one account's
  view, without touching the shared template record other accounts still use — rather than a
  delete on the template itself. Whoever implements §1/§2/§5 should decide whether that's the same
  endpoint with different internal behavior based on `IsTemplate`, or a genuinely separate
  "remove-template-from-account" action. The frontend currently calls the same `deleteX` mutation
  for both cases and just changes the tooltip text — happy to adjust the call shape once the real
  contract is decided.
- Server-side enforcement: even once the UI stops offering an Edit button for templates, the
  update endpoints (§1/§2/§5) should still reject an edit attempt on a template server-side
  (defense in depth — don't rely on the UI hiding the button).

---

## Suggested order

1. §1–§5 (Category/SubCategory/PaymentMethod update+delete, PaymentMethod kind/credit fields,
   Category avatar/color) — unblocks the screens already built in the first pass.
2. §12 (`IsTemplate` on all three outputs + the update-endpoints' server-side guard) — should land
   together with §1/§2/§5 rather than after, since it changes what "delete" means for a template.
3. §10 and §11's shared gaps (Invite cancel + member name/email/role/remove, now used by both the
   Invite members modal and the Account settings modal) — the "send invite" and "list invites"
   parts already work today.
4. §11's account-specific gaps (update name/currency, delete account, leave account, `Currency` on
   `AccountOutput`, and eventually a per-account `CurrentUserPermissionKey` so the danger-zone card
   can show one button instead of two) — frontend is fully wired and waiting.
5. §6 (Users profile endpoints) — frontend already calls these; currently silently broken.
6. §7 (Avatar upload) — needs a storage decision first.
7. §11's remaining piece, Profile revamp — needs a new sessions/revoke concept designed properly;
   not started on either side yet.
8. §8 (Chat via SignalR) — scheduled for later, not started.
9. §9 — a documentation/cleanup task, no functional urgency.
