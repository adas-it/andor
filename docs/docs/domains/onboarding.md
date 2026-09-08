# Onboarding Domain

<details>
  <summary>Sequence Diagram - Signup Flow</summary>

![Onboarding Signup Flow](https://kroki.io/plantuml/svg/eNq9VE1vEzEQve-vGPWUSC0pHDjkUDUqCCoEqRrKqZfBHjaWdu3F9qaKED8GceDEiZ-wf6zjj91kIYicuO163sy8ec-eS-fR-rauChTeWLhzbffNKgPooHVki4bDSqgGtYeTpf5o0EqlyyujvTVVRfYkQLFRY-RKlbptrkxdo5ZuRXajBEWoS9-H4G9RY0l2EZhEbJ0ODmEj6F5PGiadTm7pc0vOX8tpohQA48xAp9VKoFdGu4gSfOTGqDtm6GAG15K0V34bcSr_jKELIUyrvcsN009RBOHg7CKoMoeb5eo9zDZPZ2YQbxZFv9dfNNZ0ClSjqr4WjA5JWZ85rAIoD-u2WhQ5EkBZmBEoi13kWCQQNJgDaYG10mtkvYRVCBisdgSaBDkXDZ_mC7DLGqn65B09RBYszC1F_vGXK3IvBNH9kqo0IAmeg-x-lsobN9TcH-uF4XE12grvxupV2vRNFU2KEOiLBwOos7h2fk5LN8UA9mL5CDPuGGDDPRSSHpFmol5kkVEwFCCzoLeIW-f9Yas-sSdux_dd3OEhRG_ZQ-je6dcTrKVDTr3YKz8080PEf8XOwfUGt0aJPJBcusm13vN52SnB6_AfuUj7sA_fE7VJuNxQvv_Ymb_zH7zM5JSJFO7lyG0l9Q_vCOS-vph_F2v-CbafvNN4gKY9c8_zJ3qJ836XjFJ8BpElklavjUw6ffBtLgkLcNWfQT6be09)

</details>

## Description

The Onboarding domain owns the public *signup* flow for new users coming from the landing page,
before any authentication. It is the only module in the system with an anonymous endpoint
(`[AllowAnonymous]`) — it has to work without a JWT.

The flow has two steps: **start** the signup (name + e-mail, generates and sends a verification
code) and **verify** the signup (e-mail + code + password), which triggers the creation of the
user and the account in other domains via events.

## Endpoints

| Method | Route | Description |
|---|---|---|
| `POST` | `/v{version}/onboarding/start` | Starts (or restarts) a signup request. Generates a 6-digit verification code and publishes the `SignupCodeGenerated` event, consumed by the Communications domain to send the code by e-mail. |
| `POST` | `/v{version}/onboarding/verify` | Confirms the signup with the received code and sets the password. On success, publishes `SignupVerifiedDomainEvent`, consumed by the Users/Identity and Accounts domains so each one creates its own records. |

## Business rules

- **Code resend**: if there is already a pending (not yet verified) signup request for the given
  e-mail, `start` reuses the same `SignupRequestId` and generates a new code — the previous code
  becomes invalid automatically, since there is no time-based expiration.
- A request that has already been verified cannot be restarted or verified again (error
  `AlreadyVerified`).
- The password is never transmitted or persisted in plain text: the hash (`PasswordHasher`) is
  computed in the application service before the command reaches the domain layer or any event.
- Each signup request is processed by a dedicated actor (Akka.NET), identified by the
  `SignupRequestId` and managed by `SignupManagerActor`, guaranteeing sequential processing per
  signup.

## Domain errors

| Code | Name | When it happens |
|---|---|---|
| 8000 | `SignupNotFound` | Attempt to verify an e-mail with no signup request in progress. |
| 8001 | `InvalidCode` | The provided verification code does not match the generated one. |
| 8003 | `AlreadyVerified` | The signup request had already been confirmed. |
| 8004 | `SkippedValidations` | Informational code; does not represent a business failure. |

## Domain events

- **`SignupCodeGenerated`** — raised when the verification code is generated (or regenerated);
  consumed by the Communications domain to send the e-mail.
- **`SignupVerifiedDomainEvent`** — raised when the code is confirmed; carries a new `UserId` and
  the password hash, consumed by the Users/Identity and Accounts domains so each one creates its
  own records from the same event.

## Next steps

- Document the request/response contracts for each endpoint (`StartSignupInput`,
  `VerifySignupInput`).
- Define and document an expiration policy for the code (today it does not expire on its own, it
  is only invalidated by a new `start`).
- Link this page to the domains that consume the events (Communications, Users/Identity, Accounts)
  once they are documented too.
