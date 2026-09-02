# Onboarding Domain

<details>
  <summary>Diagrama de Sequência - Fluxo de Cadastro (Signup)</summary>

![Onboarding Signup Flow](https://kroki.io/plantuml/svg/eNq9VE1vEzEQve-vGPWUSC0pHDjkUDUqCCoEqRrKqZfBHjaWdu3F9qaKED8GceDEiZ-wf6zjj91kIYicuO163sy8ec-eS-fR-rauChTeWLhzbffNKgPooHVki4bDSqgGtYeTpf5o0EqlyyujvTVVRfYkQLFRY-RKlbptrkxdo5ZuRXajBEWoS9-H4G9RY0l2EZhEbJ0ODmEj6F5PGiadTm7pc0vOX8tpohQA48xAp9VKoFdGu4gSfOTGqDtm6GAG15K0V34bcSr_jKELIUyrvcsN009RBOHg7CKoMoeb5eo9zDZPZ2YQbxZFv9dfNNZ0ClSjqr4WjA5JWZ85rAIoD-u2WhQ5EkBZmBEoi13kWCQQNJgDaYG10mtkvYRVCBisdgSaBDkXDZ_mC7DLGqn65B09RBYszC1F_vGXK3IvBNH9kqo0IAmeg-x-lsobN9TcH-uF4XE112grvxupV2vRNFU2KEOiLBwOos7h2fk5LN8UA9mL5CDPuGGDDPRSSHpFmol5kkVEwFCCzoLeIW-f9Yas-sSdux_dd3OEhRG_ZQ-je6dcTrKVDTr3YKz8080PEf8XOwfUGt0aJPJBcusm13vN52SnB6_AfuUj7sA_fE7VJuNxQvv_Ymb_zH7zM5JSJFO7lyG0l9Q_vCOS-vph_F2v-CbafvNN4gKY9c8_zJ3qJ836XjFJ8BpElklavjUw6ffBtLgkLcNWfQT6be05)

</details>

## Descrição

O domínio Onboarding é responsável pelo fluxo público de cadastro (*signup*) de novos usuários a partir da landing page, antes de qualquer autenticação. É o único módulo do sistema com endpoint anônimo (`[AllowAnonymous]`) — precisa funcionar sem token JWT.

O fluxo acontece em duas etapas: **iniciar** o cadastro (nome + e-mail, gera e envia um código de verificação) e **confirmar** o cadastro (e-mail + código + senha), que dispara a criação do usuário e da conta em outros domínios via eventos.

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/v{version}/onboarding/start` | Inicia (ou reinicia) um pedido de cadastro. Gera um código de verificação de 6 dígitos e publica o evento `SignupCodeGenerated`, consumido pelo domínio Communications para enviar o código por e-mail. |
| `POST` | `/v{version}/onboarding/verify` | Confirma o cadastro com o código recebido e define a senha. Em caso de sucesso, publica `SignupVerifiedDomainEvent`, consumido pelos domínios Users/Identity e Accounts para cada um criar seus próprios registros. |

## Regras de negócio

- **Reenvio de código**: se já existe um pedido de cadastro pendente (ainda não verificado) para o e-mail informado, `start` reaproveita o mesmo `SignupRequestId` e gera um novo código — o código anterior fica automaticamente inválido, já que não há expiração por tempo.
- Um pedido já verificado não pode ser reiniciado nem verificado de novo (erro `AlreadyVerified`).
- A senha nunca trafega nem é persistida em texto puro: o hash (`PasswordHasher`) é calculado no serviço de aplicação antes de o comando chegar à camada de domínio ou a qualquer evento.
- Cada pedido de cadastro é processado por um ator dedicado (Akka.NET), identificado pelo `SignupRequestId` e gerenciado pelo `SignupManagerActor`, garantindo processamento sequencial por cadastro.

## Erros de domínio

| Código | Nome | Quando ocorre |
|---|---|---|
| 8000 | `SignupNotFound` | Tentativa de verificar um e-mail sem pedido de cadastro em andamento. |
| 8001 | `InvalidCode` | Código de verificação informado não confere com o gerado. |
| 8003 | `AlreadyVerified` | Pedido de cadastro já havia sido confirmado anteriormente. |
| 8004 | `SkippedValidations` | Código informativo, sem representar uma falha de negócio. |

## Eventos de domínio

- **`SignupCodeGenerated`** — emitido ao gerar (ou regenerar) o código de verificação; consumido pelo domínio Communications para o envio do e-mail.
- **`SignupVerifiedDomainEvent`** — emitido quando o código é confirmado; carrega um novo `UserId` e o hash da senha, consumido pelos domínios Users/Identity e Accounts para cada um criar seus próprios registros a partir do mesmo evento.

## Próximos passos

- Documentar os contratos de request/response de cada endpoint (`StartSignupInput`, `VerifySignupInput`).
- Definir e documentar uma política de expiração para o código (hoje ele não expira sozinho, só é invalidado por um novo `start`).
- Linkar esta página aos domínios consumidores dos eventos (Communications, Users/Identity, Accounts) quando eles também forem documentados.
