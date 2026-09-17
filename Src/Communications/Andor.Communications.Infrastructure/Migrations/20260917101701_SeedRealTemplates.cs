using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Communications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRealTemplates : Migration
    {
        // Rule already seeded by 20260902110428_SeedInitialRulesAndTemplates; only its
        // Template content is replaced here with the real HTML from templates/01-codigo-verificacao.html.
        private static readonly Guid OnboardingVerificationCodeTemplateId = new("a382b4e7-db3d-49a8-a15b-c258e86cd6d0");

        // New rule/template pair: welcome e-mail sent after signup verification.
        // RuleId matches the one already used by SignupVerifiedConsumer.
        private static readonly Guid WelcomeAfterVerificationRuleId = new("875725eb-683a-4f33-b27f-32489d127e4b");
        private static readonly Guid WelcomeAfterVerificationTemplateId = new("9f501ec1-0d01-4826-acb3-25592c4de98e");

        // New rule/template pair: account-sharing invite e-mail. No producer wired up yet.
        private static readonly Guid AccountInviteRuleId = new("ef680a94-c366-4d8a-92a4-65ed8c8fc807");
        private static readonly Guid AccountInviteTemplateId = new("4bfcd0b7-59b3-4bd9-89db-424dcef74ccd");

        // Orphaned rule/template from 20260902110428_SeedInitialRulesAndTemplates: no producer
        // ever targeted this RuleId (SignupVerifiedConsumer actually uses 875725eb-..., above).
        // Removed here; Down() restores it with its original values for reversibility.
        private static readonly Guid OldWelcomeRuleId = new("42b2aa27-82e9-4d20-8e88-afbf25e2c721");
        private static readonly Guid OldWelcomeTemplateId = new("d5b1f6c2-8a34-4e79-9c1b-2f7e0a4d6b83");

        // Fixed timestamps so the migration is deterministic across environments.
        private static readonly DateTime RuleCreatedAt = new(2026, 9, 17, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime TemplateCreatedAt = new(2026, 9, 17, 10, 17, 1, DateTimeKind.Utc);

        // Original values from 20260902110428_SeedInitialRulesAndTemplates, kept here only so
        // Down() can restore the orphaned rule/template exactly as they were.
        private static readonly DateTime OriginalRuleCreatedAt = new(2026, 9, 2, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime OriginalTemplateCreatedAt = new(2026, 8, 31, 12, 7, 30, 510, DateTimeKind.Utc);

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Communication",
                table: "Template",
                keyColumn: "Id",
                keyValue: OnboardingVerificationCodeTemplateId,
                columns: new[] { "Value", "Subject" },
                values: new object[]
                {
                    """""
<!DOCTYPE html>
<html lang="pt-BR">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta http-equiv="X-UA-Compatible" content="IE=edge">
<meta name="color-scheme" content="light dark">
<meta name="supported-color-schemes" content="light dark">
<title>Seu código de verificação Berry</title>
<!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch></o:OfficeDocumentSettings></xml><![endif]-->
<style>
  @media only screen and (max-width:620px){
    .wrap{width:100% !important}
    .pad{padding-left:24px !important;padding-right:24px !important}
    .code{font-size:34px !important;letter-spacing:8px !important}
  }
</style>
</head>
<body style="margin:0;padding:0;background-color:#F3F6F9;">
<span style="display:none;font-size:1px;color:#F3F6F9;line-height:1px;max-height:0;max-width:0;opacity:0;overflow:hidden;">Seu código de verificação é válido por 10 minutos. Use-o para confirmar seu e-mail no Berry.</span>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="background-color:#F3F6F9;">
<tr><td align="center" style="padding:32px 12px;">

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;background-color:#FFFFFF;border-radius:14px;border:1px solid #E1E8EF;">

  <tr><td class="pad" style="padding:30px 40px 0 40px;" align="left">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0"><tr>
      <td width="34" style="width:34px;"><table role="presentation" cellpadding="0" cellspacing="0" border="0" width="34"><tr><td align="center" width="34" height="34" bgcolor="#0B7A6A" style="width:34px;height:34px;border-radius:17px;color:#8FE0C8;font-family:Arial,Helvetica,sans-serif;font-size:16px;font-weight:bold;line-height:34px;mso-line-height-rule:exactly;">&bull;</td></tr></table></td>
      <td width="10" style="width:10px;">&nbsp;</td>
      <td style="font-family:Arial,Helvetica,sans-serif;font-size:19px;font-weight:bold;letter-spacing:1px;color:#16232E;">BERRY</td>
    </tr></table>
  </td></tr>

  <tr><td class="pad" style="padding:28px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:26px;line-height:32px;mso-line-height-rule:exactly;font-weight:bold;color:#16232E;">
    Confirme seu e-mail
  </td></tr>

  <tr><td class="pad" style="padding:14px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:25px;mso-line-height-rule:exactly;color:#54697A;">
    Use o código abaixo para confirmar seu endereço e continuar a criação da sua conta Berry.
  </td></tr>

  <tr><td class="pad" style="padding:26px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
      <tr><td align="center" bgcolor="#E7F4F1" style="background-color:#E7F4F1;border-radius:12px;padding:26px 16px;">
        <div class="code" style="font-family:'Courier New',Courier,monospace;font-size:40px;line-height:46px;mso-line-height-rule:exactly;font-weight:bold;letter-spacing:12px;color:#075E52;"><code></div>
        <div style="font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:18px;mso-line-height-rule:exactly;color:#4C6472;padding-top:10px;">Válido por 10 minutos</div>
      </td></tr>
    </table>
  </td></tr>

  <tr><td class="pad" style="padding:26px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:24px;mso-line-height-rule:exactly;color:#54697A;">
    Prefere não digitar? <a href="https://app.berry.finance/verificar?code=<code>" style="color:#0B7A6A;font-weight:bold;text-decoration:underline;">Confirmar com um clique</a>.
  </td></tr>

  <tr><td class="pad" style="padding:22px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;border-top:1px solid #EEF2F6;"><tr><td height="1" style="height:1px;line-height:1px;font-size:1px;">&nbsp;</td></tr></table>
  </td></tr>

  <tr><td class="pad" style="padding:18px 40px 32px 40px;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:21px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Se você não pediu este código, pode ignorar este e-mail — nenhuma conta será criada. Nunca compartilhe este código: a equipe Berry jamais vai pedi-lo.
  </td></tr>

</table>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;">
  <tr><td class="pad" align="center" style="padding:20px 40px 8px 40px;font-family:Arial,Helvetica,sans-serif;font-size:12px;line-height:19px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Berry Finanças Ltda. &middot; Rua Exemplo, 123 &middot; São Paulo/SP &middot; 01000-000<br>
    E-mail transacional relacionado à segurança da sua conta.<br>
    <a href="https://berry.finance/suporte" style="color:#7C8FA0;text-decoration:underline;">Suporte</a> &nbsp;&middot;&nbsp;
    <a href="https://berry.finance/privacidade" style="color:#7C8FA0;text-decoration:underline;">Privacidade</a>
  </td></tr>
</table>

</td></tr>
</table>
</body>
</html>
""""",
                    "Seu código de verificação Berry"
                });

            migrationBuilder.InsertData(
                schema: "Communication",
                table: "Rule",
                columns: new[] { "Id", "Name", "Type", "CreatedAt" },
                values: new object[,]
                {
                    { WelcomeAfterVerificationRuleId, "welcome-after-verification", 1, RuleCreatedAt },
                    { AccountInviteRuleId, "account-invite", 1, RuleCreatedAt }
                });

            migrationBuilder.InsertData(
                schema: "Communication",
                table: "Template",
                columns: new[] { "Id", "Value", "ContentLanguage", "Title", "Subject", "Partner", "IsDefault", "CreatedAt", "RuleId" },
                values: new object[,]
                {
                    {
                        WelcomeAfterVerificationTemplateId,
                        """""
<!DOCTYPE html>
<html lang="pt-BR">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta http-equiv="X-UA-Compatible" content="IE=edge">
<meta name="color-scheme" content="light dark">
<meta name="supported-color-schemes" content="light dark">
<title>Bem-vindo ao Berry</title>
<!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch></o:OfficeDocumentSettings></xml><![endif]-->
<style>
  @media only screen and (max-width:620px){
    .wrap{width:100% !important}
    .pad{padding-left:24px !important;padding-right:24px !important}
    .h1{font-size:26px !important;line-height:32px !important}
  }
</style>
</head>
<template id="__bundler_thumbnail" data-bg-color="#0B7A6A">
  <svg viewBox="0 0 1200 800" xmlns="http://www.w3.org/2000/svg">
    <rect width="1200" height="800" fill="#0B7A6A"/>
    <rect x="360" y="290" width="480" height="300" rx="28" fill="#FFFFFF"/>
    <path d="M360 318 L600 470 L840 318" fill="none" stroke="#0B7A6A" stroke-width="26" stroke-linejoin="round"/>
    <circle cx="840" cy="540" r="76" fill="#8FE0C8"/>
    <path d="M806 540 l24 26 46 -56" fill="none" stroke="#0B7A6A" stroke-width="20" stroke-linecap="round" stroke-linejoin="round"/>
  </svg>
</template>
<body style="margin:0;padding:0;background-color:#F3F6F9;">
<span style="display:none;font-size:1px;color:#F3F6F9;line-height:1px;max-height:0;max-width:0;opacity:0;overflow:hidden;">Sua conta está pronta. Três passos rápidos para ver o saldo do mês previsto.</span>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="background-color:#F3F6F9;">
<tr><td align="center" style="padding:32px 12px;">

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;background-color:#FFFFFF;border-radius:14px;border:1px solid #E1E8EF;">

  <tr><td bgcolor="#0B7A6A" style="background-color:#0B7A6A;border-radius:14px 14px 0 0;padding:34px 40px;" class="pad">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
      <tr><td style="font-family:Arial,Helvetica,sans-serif;font-size:17px;font-weight:bold;letter-spacing:1px;color:#8FE0C8;padding-bottom:16px;">BERRY</td></tr>
      <tr><td class="h1" style="font-family:Arial,Helvetica,sans-serif;font-size:30px;line-height:37px;mso-line-height-rule:exactly;font-weight:bold;color:#FFFFFF;">Sua conta está pronta, <name>.</td></tr>
      <tr><td style="font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:25px;mso-line-height-rule:exactly;color:#D6EDE7;padding-top:12px;">A partir de agora vocês dois acompanham o mesmo painel: lançamentos, categorias e previsão de saldo.</td></tr>
    </table>
  </td></tr>

  <tr><td class="pad" style="padding:30px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:14px;line-height:20px;mso-line-height-rule:exactly;font-weight:bold;letter-spacing:1px;color:#0B7A6A;text-transform:uppercase;">
    Comece por aqui
  </td></tr>

  <tr><td class="pad" style="padding:16px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
      <tr>
        <td width="34" valign="top" style="width:34px;"><table role="presentation" cellpadding="0" cellspacing="0" border="0" width="28"><tr><td align="center" width="28" height="28" bgcolor="#E7F4F1" style="width:28px;height:28px;border-radius:14px;font-family:Arial,Helvetica,sans-serif;font-size:13px;font-weight:bold;color:#0B7A6A;line-height:28px;mso-line-height-rule:exactly;">1</td></tr></table></td>
        <td valign="top" style="font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:23px;mso-line-height-rule:exactly;color:#31424F;padding-bottom:16px;"><strong style="color:#16232E;">Lance a primeira despesa.</strong><br><span style="color:#5C7182;">Tipo, categoria e valor — leva uns quinze segundos.</span></td>
      </tr>
      <tr>
        <td width="34" valign="top" style="width:34px;"><table role="presentation" cellpadding="0" cellspacing="0" border="0" width="28"><tr><td align="center" width="28" height="28" bgcolor="#FDE9E2" style="width:28px;height:28px;border-radius:14px;font-family:Arial,Helvetica,sans-serif;font-size:13px;font-weight:bold;color:#E9714F;line-height:28px;mso-line-height-rule:exactly;">2</td></tr></table></td>
        <td valign="top" style="font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:23px;mso-line-height-rule:exactly;color:#31424F;padding-bottom:16px;"><strong style="color:#16232E;">Cadastre as contas fixas.</strong><br><span style="color:#5C7182;">Recorrentes se repetem sozinhas e já entram na previsão.</span></td>
      </tr>
      <tr>
        <td width="34" valign="top" style="width:34px;"><table role="presentation" cellpadding="0" cellspacing="0" border="0" width="28"><tr><td align="center" width="28" height="28" bgcolor="#EAF0F5" style="width:28px;height:28px;border-radius:14px;font-family:Arial,Helvetica,sans-serif;font-size:13px;font-weight:bold;color:#4C6472;line-height:28px;mso-line-height-rule:exactly;">3</td></tr></table></td>
        <td valign="top" style="font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:23px;mso-line-height-rule:exactly;color:#31424F;"><strong style="color:#16232E;">Convide sua dupla.</strong><br><span style="color:#5C7182;">Dois perfis, um painel só. Sem planilha compartilhada.</span></td>
      </tr>
    </table>
  </td></tr>

  <tr><td class="pad" style="padding:28px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0"><tr>
      <td align="center" bgcolor="#0B7A6A" style="background-color:#0B7A6A;border-radius:9px;">
        <a href="https://app.berry.finance/dashboard" style="display:block;padding:15px 30px;font-family:Arial,Helvetica,sans-serif;font-size:16px;font-weight:bold;line-height:20px;mso-line-height-rule:exactly;color:#FFFFFF;text-decoration:none;">Abrir meu painel</a>
      </td>
    </tr></table>
  </td></tr>

  <tr><td class="pad" style="padding:26px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
      <tr><td bgcolor="#F7F9FB" style="background-color:#F7F9FB;border-radius:12px;padding:20px 22px;font-family:Arial,Helvetica,sans-serif;font-size:14px;line-height:22px;mso-line-height-rule:exactly;color:#54697A;">
        <strong style="color:#16232E;">Dica:</strong> o Berry nunca pede o login do seu banco. Tudo que aparece no painel foi lançado por você ou pela sua dupla.
      </td></tr>
    </table>
  </td></tr>

  <tr><td class="pad" style="padding:24px 40px 32px 40px;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:21px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Dúvidas? Responda este e-mail ou fale com a gente em <a href="https://berry.finance/suporte" style="color:#0B7A6A;text-decoration:underline;">berry.finance/suporte</a>.
  </td></tr>

</table>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;">
  <tr><td class="pad" align="center" style="padding:20px 40px 8px 40px;font-family:Arial,Helvetica,sans-serif;font-size:12px;line-height:19px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Berry Finanças Ltda. &middot; Rua Exemplo, 123 &middot; São Paulo/SP &middot; 01000-000<br>
    <a href="https://berry.finance/preferencias" style="color:#7C8FA0;text-decoration:underline;">Preferências de e-mail</a> &nbsp;&middot;&nbsp;
    <a href="https://berry.finance/cancelar" style="color:#7C8FA0;text-decoration:underline;">Cancelar inscrição</a> &nbsp;&middot;&nbsp;
    <a href="https://berry.finance/privacidade" style="color:#7C8FA0;text-decoration:underline;">Privacidade</a>
  </td></tr>
</table>

</td></tr>
</table>
</body>
</html>
""""",
                        "en",
                        "wellcome",
                        "Bem-vindo ao Berry",
                        1,
                        true,
                        TemplateCreatedAt,
                        WelcomeAfterVerificationRuleId
                    },
                    {
                        AccountInviteTemplateId,
                        """""
<!DOCTYPE html>
<html lang="pt-BR">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta http-equiv="X-UA-Compatible" content="IE=edge">
<meta name="color-scheme" content="light dark">
<meta name="supported-color-schemes" content="light dark">
<title>Você foi convidado para uma conta no Berry</title>
<!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch></o:OfficeDocumentSettings></xml><![endif]-->
<style>
  @media only screen and (max-width:620px){
    .wrap{width:100% !important}
    .pad{padding-left:24px !important;padding-right:24px !important}
    .h1{font-size:25px !important;line-height:31px !important}
  }
</style>
</head>
<body style="margin:0;padding:0;background-color:#F3F6F9;">
<span style="display:none;font-size:1px;color:#F3F6F9;line-height:1px;max-height:0;max-width:0;opacity:0;overflow:hidden;"><inviter_name> convidou você para a conta "<account_name>" no Berry. O convite expira em 7 dias.</span>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="background-color:#F3F6F9;">
<tr><td align="center" style="padding:32px 12px;">

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;background-color:#FFFFFF;border-radius:14px;border:1px solid #E1E8EF;">

  <tr><td class="pad" style="padding:30px 40px 0 40px;" align="left">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0"><tr>
      <td width="34" style="width:34px;"><table role="presentation" cellpadding="0" cellspacing="0" border="0" width="34"><tr><td align="center" width="34" height="34" bgcolor="#0B7A6A" style="width:34px;height:34px;border-radius:17px;color:#8FE0C8;font-family:Arial,Helvetica,sans-serif;font-size:16px;font-weight:bold;line-height:34px;mso-line-height-rule:exactly;">&bull;</td></tr></table></td>
      <td width="10" style="width:10px;">&nbsp;</td>
      <td style="font-family:Arial,Helvetica,sans-serif;font-size:19px;font-weight:bold;letter-spacing:1px;color:#16232E;">BERRY</td>
    </tr></table>
  </td></tr>

  <tr><td class="pad h1" style="padding:28px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:28px;line-height:35px;mso-line-height-rule:exactly;font-weight:bold;color:#16232E;">
    <inviter_name> convidou você para cuidar das contas juntos
  </td></tr>

  <tr><td class="pad" style="padding:14px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:25px;mso-line-height-rule:exactly;color:#54697A;">
    Ao aceitar, você passa a ver e lançar movimentações na conta compartilhada — com o mesmo saldo, as mesmas categorias e a mesma previsão de fim de mês.
  </td></tr>

  <tr><td class="pad" style="padding:24px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
      <tr><td bgcolor="#F7F9FB" style="background-color:#F7F9FB;border-radius:12px;border:1px solid #E7EDF3;padding:22px 24px;">
        <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
          <tr>
            <td width="44" valign="middle" style="width:44px;"><table role="presentation" cellpadding="0" cellspacing="0" border="0" width="44"><tr><td align="center" width="44" height="44" bgcolor="#E7F4F1" style="width:44px;height:44px;border-radius:22px;font-family:Arial,Helvetica,sans-serif;font-size:17px;font-weight:bold;color:#0B7A6A;line-height:44px;mso-line-height-rule:exactly;"><inviter_initials></td></tr></table></td>
            <td width="14" style="width:14px;">&nbsp;</td>
            <td valign="middle" style="font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:22px;mso-line-height-rule:exactly;color:#16232E;">
              <strong><inviter_full_name></strong><br><span style="font-size:13px;color:#7C8FA0;"><inviter_email></span>
            </td>
          </tr>
          <tr><td colspan="3" height="18" style="height:18px;line-height:18px;font-size:1px;">&nbsp;</td></tr>
          <tr><td colspan="3" style="border-top:1px solid #E7EDF3;font-size:1px;line-height:1px;height:1px;">&nbsp;</td></tr>
          <tr><td colspan="3" height="16" style="height:16px;line-height:16px;font-size:1px;">&nbsp;</td></tr>
          <tr><td colspan="3">
            <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
              <tr>
                <td width="50%" style="width:50%;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:20px;mso-line-height-rule:exactly;color:#7C8FA0;">Conta<br><span style="font-size:15px;font-weight:bold;color:#16232E;"><account_name></span></td>
                <td width="50%" style="width:50%;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:20px;mso-line-height-rule:exactly;color:#7C8FA0;">Seu acesso<br><span style="font-size:15px;font-weight:bold;color:#16232E;"><access_level></span></td>
              </tr>
            </table>
          </td></tr>
        </table>
      </td></tr>
    </table>
  </td></tr>

  <tr><td class="pad" style="padding:26px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0"><tr>
      <td align="center" bgcolor="#0B7A6A" style="background-color:#0B7A6A;border-radius:9px;">
        <a href="<accept_url>" style="display:block;padding:15px 30px;font-family:Arial,Helvetica,sans-serif;font-size:16px;font-weight:bold;line-height:20px;mso-line-height-rule:exactly;color:#FFFFFF;text-decoration:none;">Aceitar convite</a>
      </td>
      <td width="12" style="width:12px;">&nbsp;</td>
      <td align="center" bgcolor="#FFFFFF" style="background-color:#FFFFFF;border:1px solid #D7E0E8;border-radius:9px;">
        <a href="<decline_url>" style="display:block;padding:14px 24px;font-family:Arial,Helvetica,sans-serif;font-size:16px;font-weight:bold;line-height:20px;mso-line-height-rule:exactly;color:#16232E;text-decoration:none;">Recusar</a>
      </td>
    </tr></table>
  </td></tr>

  <tr><td class="pad" style="padding:18px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:21px;mso-line-height-rule:exactly;color:#7C8FA0;">
    O convite expira em 7 dias. Se o botão não funcionar, copie este endereço:<br>
    <a href="<accept_url>" style="color:#0B7A6A;text-decoration:underline;word-break:break-all;"><accept_url></a>
  </td></tr>

  <tr><td class="pad" style="padding:22px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;border-top:1px solid #EEF2F6;"><tr><td height="1" style="height:1px;line-height:1px;font-size:1px;">&nbsp;</td></tr></table>
  </td></tr>

  <tr><td class="pad" style="padding:18px 40px 32px 40px;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:21px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Não conhece <inviter_name>? Ignore este e-mail — sem a sua confirmação, ninguém tem acesso a nada seu.
  </td></tr>

</table>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;">
  <tr><td class="pad" align="center" style="padding:20px 40px 8px 40px;font-family:Arial,Helvetica,sans-serif;font-size:12px;line-height:19px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Berry Finanças Ltda. &middot; Rua Exemplo, 123 &middot; São Paulo/SP &middot; 01000-000<br>
    <a href="https://berry.finance/suporte" style="color:#7C8FA0;text-decoration:underline;">Suporte</a> &nbsp;&middot;&nbsp;
    <a href="https://berry.finance/privacidade" style="color:#7C8FA0;text-decoration:underline;">Privacidade</a>
  </td></tr>
</table>

</td></tr>
</table>
</body>
</html>
""""",
                        "en",
                        "wellcome",
                        "Você foi convidado para uma conta no Berry",
                        1,
                        true,
                        TemplateCreatedAt,
                        AccountInviteRuleId
                    }
                });

            migrationBuilder.DeleteData(
                schema: "Communication",
                table: "Template",
                keyColumn: "Id",
                keyValue: OldWelcomeTemplateId);

            migrationBuilder.DeleteData(
                schema: "Communication",
                table: "Rule",
                keyColumn: "Id",
                keyValue: OldWelcomeRuleId);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Communication",
                table: "Rule",
                columns: new[] { "Id", "Name", "Type", "CreatedAt" },
                values: new object[] { OldWelcomeRuleId, "wellcome", 1, OriginalRuleCreatedAt });

            migrationBuilder.InsertData(
                schema: "Communication",
                table: "Template",
                columns: new[] { "Id", "Value", "ContentLanguage", "Title", "Subject", "Partner", "IsDefault", "CreatedAt", "RuleId" },
                values: new object[]
                {
                    OldWelcomeTemplateId,
                    "<h1>Welcome <name>!</h1><br>We're glad to have you on board.",
                    "en",
                    "wellcome",
                    "Welcome Email",
                    1,
                    true,
                    OriginalTemplateCreatedAt,
                    OldWelcomeRuleId
                });

            migrationBuilder.DeleteData(
                schema: "Communication",
                table: "Template",
                keyColumn: "Id",
                keyValues: new object[] { WelcomeAfterVerificationTemplateId, AccountInviteTemplateId });

            migrationBuilder.DeleteData(
                schema: "Communication",
                table: "Rule",
                keyColumn: "Id",
                keyValues: new object[] { WelcomeAfterVerificationRuleId, AccountInviteRuleId });

            migrationBuilder.UpdateData(
                schema: "Communication",
                table: "Template",
                keyColumn: "Id",
                keyValue: OnboardingVerificationCodeTemplateId,
                columns: new[] { "Value", "Subject" },
                values: new object[]
                {
                    "<h1>Hello <name>!</h1><br>Your code is <code>",
                    "Welcome Email"
                });
        }
    }
}
