using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Andor.Communications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedMultilingualTemplates : Migration
    {
        // The three "wellcome" templates seeded by 20260917101701_SeedRealTemplates were tagged
        // ContentLanguage = "en" but their actual copy is Portuguese. Relabel them to "br" (the
        // ISO code Andor.Shared.Lookups.Language uses for Portuguese) and add real "en"/"es"
        // translations alongside them, so RuleActor's Title+ContentLanguage lookup can actually
        // serve all three languages instead of only ever matching the (mislabeled) Portuguese copy.
        private static readonly Guid OnboardingVerificationCodeTemplateId = new("a382b4e7-db3d-49a8-a15b-c258e86cd6d0");
        private static readonly Guid WelcomeAfterVerificationTemplateId = new("9f501ec1-0d01-4826-acb3-25592c4de98e");
        private static readonly Guid AccountInviteTemplateId = new("4bfcd0b7-59b3-4bd9-89db-424dcef74ccd");

        private static readonly Guid OnboardingVerificationCodeRuleId = new("acb860a5-1af6-4b03-afae-e290dfcac7d4");
        private static readonly Guid WelcomeAfterVerificationRuleId = new("875725eb-683a-4f33-b27f-32489d127e4b");
        private static readonly Guid AccountInviteRuleId = new("ef680a94-c366-4d8a-92a4-65ed8c8fc807");

        private static readonly Guid OnboardingVerificationCodeEnTemplateId = new("1e0a3b4c-2f11-4a6e-9c3d-7a1b2c3d4e01");
        private static readonly Guid OnboardingVerificationCodeEsTemplateId = new("2e0a3b4c-2f11-4a6e-9c3d-7a1b2c3d4e02");
        private static readonly Guid WelcomeAfterVerificationEnTemplateId = new("1e0a3b4c-2f11-4a6e-9c3d-7a1b2c3d4e03");
        private static readonly Guid WelcomeAfterVerificationEsTemplateId = new("2e0a3b4c-2f11-4a6e-9c3d-7a1b2c3d4e04");
        private static readonly Guid AccountInviteEnTemplateId = new("1e0a3b4c-2f11-4a6e-9c3d-7a1b2c3d4e05");
        private static readonly Guid AccountInviteEsTemplateId = new("2e0a3b4c-2f11-4a6e-9c3d-7a1b2c3d4e06");

        // Fixed timestamp so the migration is deterministic across environments.
        private static readonly DateTime TemplateCreatedAt = new(2026, 9, 22, 18, 21, 22, DateTimeKind.Utc);

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Communication",
                table: "Template",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    OnboardingVerificationCodeTemplateId,
                    WelcomeAfterVerificationTemplateId,
                    AccountInviteTemplateId,
                },
                column: "ContentLanguage",
                values: new object[] { "br", "br", "br" });

            migrationBuilder.InsertData(
                schema: "Communication",
                table: "Template",
                columns: new[] { "Id", "Value", "ContentLanguage", "Title", "Subject", "Partner", "IsDefault", "CreatedAt", "RuleId" },
                values: new object[,]
                {
                    {
                        OnboardingVerificationCodeEnTemplateId,
                        """""
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta http-equiv="X-UA-Compatible" content="IE=edge">
<meta name="color-scheme" content="light dark">
<meta name="supported-color-schemes" content="light dark">
<title>Your Berry verification code</title>
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
<span style="display:none;font-size:1px;color:#F3F6F9;line-height:1px;max-height:0;max-width:0;opacity:0;overflow:hidden;">Your verification code is valid for 10 minutes. Use it to confirm your email on Berry.</span>

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
    Confirm your email
  </td></tr>

  <tr><td class="pad" style="padding:14px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:25px;mso-line-height-rule:exactly;color:#54697A;">
    Use the code below to confirm your address and continue creating your Berry account.
  </td></tr>

  <tr><td class="pad" style="padding:26px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
      <tr><td align="center" bgcolor="#E7F4F1" style="background-color:#E7F4F1;border-radius:12px;padding:26px 16px;">
        <div class="code" style="font-family:'Courier New',Courier,monospace;font-size:40px;line-height:46px;mso-line-height-rule:exactly;font-weight:bold;letter-spacing:12px;color:#075E52;"><code></div>
        <div style="font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:18px;mso-line-height-rule:exactly;color:#4C6472;padding-top:10px;">Valid for 10 minutes</div>
      </td></tr>
    </table>
  </td></tr>

  <tr><td class="pad" style="padding:26px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:24px;mso-line-height-rule:exactly;color:#54697A;">
    Prefer not to type it? <a href="https://app.berry.finance/verificar?code=<code>" style="color:#0B7A6A;font-weight:bold;text-decoration:underline;">Confirm with one click</a>.
  </td></tr>

  <tr><td class="pad" style="padding:22px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;border-top:1px solid #EEF2F6;"><tr><td height="1" style="height:1px;line-height:1px;font-size:1px;">&nbsp;</td></tr></table>
  </td></tr>

  <tr><td class="pad" style="padding:18px 40px 32px 40px;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:21px;mso-line-height-rule:exactly;color:#7C8FA0;">
    If you didn't request this code, you can ignore this email — no account will be created. Never share this code: the Berry team will never ask for it.
  </td></tr>

</table>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;">
  <tr><td class="pad" align="center" style="padding:20px 40px 8px 40px;font-family:Arial,Helvetica,sans-serif;font-size:12px;line-height:19px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Berry Finanças Ltda. &middot; Rua Exemplo, 123 &middot; São Paulo/SP &middot; 01000-000<br>
    Transactional email related to your account's security.<br>
    <a href="https://berry.finance/suporte" style="color:#7C8FA0;text-decoration:underline;">Support</a> &nbsp;&middot;&nbsp;
    <a href="https://berry.finance/privacidade" style="color:#7C8FA0;text-decoration:underline;">Privacy</a>
  </td></tr>
</table>

</td></tr>
</table>
</body>
</html>
""""",
                        "en",
                        "wellcome",
                        "Your Berry verification code",
                        1,
                        true,
                        TemplateCreatedAt,
                        OnboardingVerificationCodeRuleId
                    },
                    {
                        OnboardingVerificationCodeEsTemplateId,
                        """""
<!DOCTYPE html>
<html lang="es">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta http-equiv="X-UA-Compatible" content="IE=edge">
<meta name="color-scheme" content="light dark">
<meta name="supported-color-schemes" content="light dark">
<title>Tu código de verificación de Berry</title>
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
<span style="display:none;font-size:1px;color:#F3F6F9;line-height:1px;max-height:0;max-width:0;opacity:0;overflow:hidden;">Tu código de verificación es válido durante 10 minutos. Úsalo para confirmar tu correo en Berry.</span>

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
    Confirma tu correo electrónico
  </td></tr>

  <tr><td class="pad" style="padding:14px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:25px;mso-line-height-rule:exactly;color:#54697A;">
    Usa el código de abajo para confirmar tu dirección y continuar la creación de tu cuenta Berry.
  </td></tr>

  <tr><td class="pad" style="padding:26px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
      <tr><td align="center" bgcolor="#E7F4F1" style="background-color:#E7F4F1;border-radius:12px;padding:26px 16px;">
        <div class="code" style="font-family:'Courier New',Courier,monospace;font-size:40px;line-height:46px;mso-line-height-rule:exactly;font-weight:bold;letter-spacing:12px;color:#075E52;"><code></div>
        <div style="font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:18px;mso-line-height-rule:exactly;color:#4C6472;padding-top:10px;">Válido durante 10 minutos</div>
      </td></tr>
    </table>
  </td></tr>

  <tr><td class="pad" style="padding:26px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:24px;mso-line-height-rule:exactly;color:#54697A;">
    ¿Prefieres no escribirlo? <a href="https://app.berry.finance/verificar?code=<code>" style="color:#0B7A6A;font-weight:bold;text-decoration:underline;">Confirmar con un clic</a>.
  </td></tr>

  <tr><td class="pad" style="padding:22px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;border-top:1px solid #EEF2F6;"><tr><td height="1" style="height:1px;line-height:1px;font-size:1px;">&nbsp;</td></tr></table>
  </td></tr>

  <tr><td class="pad" style="padding:18px 40px 32px 40px;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:21px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Si no solicitaste este código, puedes ignorar este correo — no se creará ninguna cuenta. Nunca compartas este código: el equipo de Berry nunca te lo pedirá.
  </td></tr>

</table>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;">
  <tr><td class="pad" align="center" style="padding:20px 40px 8px 40px;font-family:Arial,Helvetica,sans-serif;font-size:12px;line-height:19px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Berry Finanças Ltda. &middot; Rua Exemplo, 123 &middot; São Paulo/SP &middot; 01000-000<br>
    Correo transaccional relacionado con la seguridad de tu cuenta.<br>
    <a href="https://berry.finance/suporte" style="color:#7C8FA0;text-decoration:underline;">Soporte</a> &nbsp;&middot;&nbsp;
    <a href="https://berry.finance/privacidade" style="color:#7C8FA0;text-decoration:underline;">Privacidad</a>
  </td></tr>
</table>

</td></tr>
</table>
</body>
</html>
""""",
                        "es",
                        "wellcome",
                        "Tu código de verificación de Berry",
                        1,
                        true,
                        TemplateCreatedAt,
                        OnboardingVerificationCodeRuleId
                    },
                    {
                        WelcomeAfterVerificationEnTemplateId,
                        """""
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta http-equiv="X-UA-Compatible" content="IE=edge">
<meta name="color-scheme" content="light dark">
<meta name="supported-color-schemes" content="light dark">
<title>Welcome to Berry</title>
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
<span style="display:none;font-size:1px;color:#F3F6F9;line-height:1px;max-height:0;max-width:0;opacity:0;overflow:hidden;">Your account is ready. Three quick steps to see your projected month-end balance.</span>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="background-color:#F3F6F9;">
<tr><td align="center" style="padding:32px 12px;">

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;background-color:#FFFFFF;border-radius:14px;border:1px solid #E1E8EF;">

  <tr><td bgcolor="#0B7A6A" style="background-color:#0B7A6A;border-radius:14px 14px 0 0;padding:34px 40px;" class="pad">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
      <tr><td style="font-family:Arial,Helvetica,sans-serif;font-size:17px;font-weight:bold;letter-spacing:1px;color:#8FE0C8;padding-bottom:16px;">BERRY</td></tr>
      <tr><td class="h1" style="font-family:Arial,Helvetica,sans-serif;font-size:30px;line-height:37px;mso-line-height-rule:exactly;font-weight:bold;color:#FFFFFF;">Your account is ready, <name>.</td></tr>
      <tr><td style="font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:25px;mso-line-height-rule:exactly;color:#D6EDE7;padding-top:12px;">From now on you both follow the same dashboard: transactions, categories and balance forecast.</td></tr>
    </table>
  </td></tr>

  <tr><td class="pad" style="padding:30px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:14px;line-height:20px;mso-line-height-rule:exactly;font-weight:bold;letter-spacing:1px;color:#0B7A6A;text-transform:uppercase;">
    Start here
  </td></tr>

  <tr><td class="pad" style="padding:16px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
      <tr>
        <td width="34" valign="top" style="width:34px;"><table role="presentation" cellpadding="0" cellspacing="0" border="0" width="28"><tr><td align="center" width="28" height="28" bgcolor="#E7F4F1" style="width:28px;height:28px;border-radius:14px;font-family:Arial,Helvetica,sans-serif;font-size:13px;font-weight:bold;color:#0B7A6A;line-height:28px;mso-line-height-rule:exactly;">1</td></tr></table></td>
        <td valign="top" style="font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:23px;mso-line-height-rule:exactly;color:#31424F;padding-bottom:16px;"><strong style="color:#16232E;">Log your first expense.</strong><br><span style="color:#5C7182;">Type, category and amount — takes about fifteen seconds.</span></td>
      </tr>
      <tr>
        <td width="34" valign="top" style="width:34px;"><table role="presentation" cellpadding="0" cellspacing="0" border="0" width="28"><tr><td align="center" width="28" height="28" bgcolor="#FDE9E2" style="width:28px;height:28px;border-radius:14px;font-family:Arial,Helvetica,sans-serif;font-size:13px;font-weight:bold;color:#E9714F;line-height:28px;mso-line-height-rule:exactly;">2</td></tr></table></td>
        <td valign="top" style="font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:23px;mso-line-height-rule:exactly;color:#31424F;padding-bottom:16px;"><strong style="color:#16232E;">Set up your fixed bills.</strong><br><span style="color:#5C7182;">Recurring ones repeat automatically and already count toward the forecast.</span></td>
      </tr>
      <tr>
        <td width="34" valign="top" style="width:34px;"><table role="presentation" cellpadding="0" cellspacing="0" border="0" width="28"><tr><td align="center" width="28" height="28" bgcolor="#EAF0F5" style="width:28px;height:28px;border-radius:14px;font-family:Arial,Helvetica,sans-serif;font-size:13px;font-weight:bold;color:#4C6472;line-height:28px;mso-line-height-rule:exactly;">3</td></tr></table></td>
        <td valign="top" style="font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:23px;mso-line-height-rule:exactly;color:#31424F;"><strong style="color:#16232E;">Invite your partner.</strong><br><span style="color:#5C7182;">Two profiles, one dashboard. No shared spreadsheet.</span></td>
      </tr>
    </table>
  </td></tr>

  <tr><td class="pad" style="padding:28px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0"><tr>
      <td align="center" bgcolor="#0B7A6A" style="background-color:#0B7A6A;border-radius:9px;">
        <a href="https://app.berry.finance/dashboard" style="display:block;padding:15px 30px;font-family:Arial,Helvetica,sans-serif;font-size:16px;font-weight:bold;line-height:20px;mso-line-height-rule:exactly;color:#FFFFFF;text-decoration:none;">Open my dashboard</a>
      </td>
    </tr></table>
  </td></tr>

  <tr><td class="pad" style="padding:26px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
      <tr><td bgcolor="#F7F9FB" style="background-color:#F7F9FB;border-radius:12px;padding:20px 22px;font-family:Arial,Helvetica,sans-serif;font-size:14px;line-height:22px;mso-line-height-rule:exactly;color:#54697A;">
        <strong style="color:#16232E;">Tip:</strong> Berry never asks for your bank login. Everything on the dashboard was entered by you or your partner.
      </td></tr>
    </table>
  </td></tr>

  <tr><td class="pad" style="padding:24px 40px 32px 40px;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:21px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Questions? Reply to this email or reach us at <a href="https://berry.finance/suporte" style="color:#0B7A6A;text-decoration:underline;">berry.finance/suporte</a>.
  </td></tr>

</table>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;">
  <tr><td class="pad" align="center" style="padding:20px 40px 8px 40px;font-family:Arial,Helvetica,sans-serif;font-size:12px;line-height:19px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Berry Finanças Ltda. &middot; Rua Exemplo, 123 &middot; São Paulo/SP &middot; 01000-000<br>
    <a href="https://berry.finance/preferencias" style="color:#7C8FA0;text-decoration:underline;">Email preferences</a> &nbsp;&middot;&nbsp;
    <a href="https://berry.finance/cancelar" style="color:#7C8FA0;text-decoration:underline;">Unsubscribe</a> &nbsp;&middot;&nbsp;
    <a href="https://berry.finance/privacidade" style="color:#7C8FA0;text-decoration:underline;">Privacy</a>
  </td></tr>
</table>

</td></tr>
</table>
</body>
</html>
""""",
                        "en",
                        "wellcome",
                        "Welcome to Berry",
                        1,
                        true,
                        TemplateCreatedAt,
                        WelcomeAfterVerificationRuleId
                    },
                    {
                        WelcomeAfterVerificationEsTemplateId,
                        """""
<!DOCTYPE html>
<html lang="es">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta http-equiv="X-UA-Compatible" content="IE=edge">
<meta name="color-scheme" content="light dark">
<meta name="supported-color-schemes" content="light dark">
<title>Bienvenido a Berry</title>
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
<span style="display:none;font-size:1px;color:#F3F6F9;line-height:1px;max-height:0;max-width:0;opacity:0;overflow:hidden;">Tu cuenta está lista. Tres pasos rápidos para ver el saldo previsto de fin de mes.</span>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="background-color:#F3F6F9;">
<tr><td align="center" style="padding:32px 12px;">

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;background-color:#FFFFFF;border-radius:14px;border:1px solid #E1E8EF;">

  <tr><td bgcolor="#0B7A6A" style="background-color:#0B7A6A;border-radius:14px 14px 0 0;padding:34px 40px;" class="pad">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
      <tr><td style="font-family:Arial,Helvetica,sans-serif;font-size:17px;font-weight:bold;letter-spacing:1px;color:#8FE0C8;padding-bottom:16px;">BERRY</td></tr>
      <tr><td class="h1" style="font-family:Arial,Helvetica,sans-serif;font-size:30px;line-height:37px;mso-line-height-rule:exactly;font-weight:bold;color:#FFFFFF;">Tu cuenta está lista, <name>.</td></tr>
      <tr><td style="font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:25px;mso-line-height-rule:exactly;color:#D6EDE7;padding-top:12px;">A partir de ahora ambos siguen el mismo panel: movimientos, categorías y previsión de saldo.</td></tr>
    </table>
  </td></tr>

  <tr><td class="pad" style="padding:30px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:14px;line-height:20px;mso-line-height-rule:exactly;font-weight:bold;letter-spacing:1px;color:#0B7A6A;text-transform:uppercase;">
    Empieza por aquí
  </td></tr>

  <tr><td class="pad" style="padding:16px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
      <tr>
        <td width="34" valign="top" style="width:34px;"><table role="presentation" cellpadding="0" cellspacing="0" border="0" width="28"><tr><td align="center" width="28" height="28" bgcolor="#E7F4F1" style="width:28px;height:28px;border-radius:14px;font-family:Arial,Helvetica,sans-serif;font-size:13px;font-weight:bold;color:#0B7A6A;line-height:28px;mso-line-height-rule:exactly;">1</td></tr></table></td>
        <td valign="top" style="font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:23px;mso-line-height-rule:exactly;color:#31424F;padding-bottom:16px;"><strong style="color:#16232E;">Registra tu primer gasto.</strong><br><span style="color:#5C7182;">Tipo, categoría e importe — toma unos quince segundos.</span></td>
      </tr>
      <tr>
        <td width="34" valign="top" style="width:34px;"><table role="presentation" cellpadding="0" cellspacing="0" border="0" width="28"><tr><td align="center" width="28" height="28" bgcolor="#FDE9E2" style="width:28px;height:28px;border-radius:14px;font-family:Arial,Helvetica,sans-serif;font-size:13px;font-weight:bold;color:#E9714F;line-height:28px;mso-line-height-rule:exactly;">2</td></tr></table></td>
        <td valign="top" style="font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:23px;mso-line-height-rule:exactly;color:#31424F;padding-bottom:16px;"><strong style="color:#16232E;">Registra tus cuentas fijas.</strong><br><span style="color:#5C7182;">Las recurrentes se repiten solas y ya entran en la previsión.</span></td>
      </tr>
      <tr>
        <td width="34" valign="top" style="width:34px;"><table role="presentation" cellpadding="0" cellspacing="0" border="0" width="28"><tr><td align="center" width="28" height="28" bgcolor="#EAF0F5" style="width:28px;height:28px;border-radius:14px;font-family:Arial,Helvetica,sans-serif;font-size:13px;font-weight:bold;color:#4C6472;line-height:28px;mso-line-height-rule:exactly;">3</td></tr></table></td>
        <td valign="top" style="font-family:Arial,Helvetica,sans-serif;font-size:15px;line-height:23px;mso-line-height-rule:exactly;color:#31424F;"><strong style="color:#16232E;">Invita a tu pareja.</strong><br><span style="color:#5C7182;">Dos perfiles, un solo panel. Sin hoja de cálculo compartida.</span></td>
      </tr>
    </table>
  </td></tr>

  <tr><td class="pad" style="padding:28px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0"><tr>
      <td align="center" bgcolor="#0B7A6A" style="background-color:#0B7A6A;border-radius:9px;">
        <a href="https://app.berry.finance/dashboard" style="display:block;padding:15px 30px;font-family:Arial,Helvetica,sans-serif;font-size:16px;font-weight:bold;line-height:20px;mso-line-height-rule:exactly;color:#FFFFFF;text-decoration:none;">Abrir mi panel</a>
      </td>
    </tr></table>
  </td></tr>

  <tr><td class="pad" style="padding:26px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;">
      <tr><td bgcolor="#F7F9FB" style="background-color:#F7F9FB;border-radius:12px;padding:20px 22px;font-family:Arial,Helvetica,sans-serif;font-size:14px;line-height:22px;mso-line-height-rule:exactly;color:#54697A;">
        <strong style="color:#16232E;">Consejo:</strong> Berry nunca pide el acceso a tu banco. Todo lo que aparece en el panel fue registrado por ti o por tu pareja.
      </td></tr>
    </table>
  </td></tr>

  <tr><td class="pad" style="padding:24px 40px 32px 40px;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:21px;mso-line-height-rule:exactly;color:#7C8FA0;">
    ¿Dudas? Responde este correo o escríbenos en <a href="https://berry.finance/suporte" style="color:#0B7A6A;text-decoration:underline;">berry.finance/suporte</a>.
  </td></tr>

</table>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;">
  <tr><td class="pad" align="center" style="padding:20px 40px 8px 40px;font-family:Arial,Helvetica,sans-serif;font-size:12px;line-height:19px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Berry Finanças Ltda. &middot; Rua Exemplo, 123 &middot; São Paulo/SP &middot; 01000-000<br>
    <a href="https://berry.finance/preferencias" style="color:#7C8FA0;text-decoration:underline;">Preferencias de correo</a> &nbsp;&middot;&nbsp;
    <a href="https://berry.finance/cancelar" style="color:#7C8FA0;text-decoration:underline;">Cancelar suscripción</a> &nbsp;&middot;&nbsp;
    <a href="https://berry.finance/privacidade" style="color:#7C8FA0;text-decoration:underline;">Privacidad</a>
  </td></tr>
</table>

</td></tr>
</table>
</body>
</html>
""""",
                        "es",
                        "wellcome",
                        "Bienvenido a Berry",
                        1,
                        true,
                        TemplateCreatedAt,
                        WelcomeAfterVerificationRuleId
                    },
                    {
                        AccountInviteEnTemplateId,
                        """""
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta http-equiv="X-UA-Compatible" content="IE=edge">
<meta name="color-scheme" content="light dark">
<meta name="supported-color-schemes" content="light dark">
<title>You've been invited to a Berry account</title>
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
<span style="display:none;font-size:1px;color:#F3F6F9;line-height:1px;max-height:0;max-width:0;opacity:0;overflow:hidden;"><inviter_name> invited you to the "<account_name>" account on Berry. The invite expires in 7 days.</span>

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
    <inviter_name> invited you to manage accounts together
  </td></tr>

  <tr><td class="pad" style="padding:14px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:25px;mso-line-height-rule:exactly;color:#54697A;">
    By accepting, you'll be able to see and log transactions on the shared account — with the same balance, the same categories and the same month-end forecast.
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
                <td width="50%" style="width:50%;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:20px;mso-line-height-rule:exactly;color:#7C8FA0;">Account<br><span style="font-size:15px;font-weight:bold;color:#16232E;"><account_name></span></td>
                <td width="50%" style="width:50%;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:20px;mso-line-height-rule:exactly;color:#7C8FA0;">Your access<br><span style="font-size:15px;font-weight:bold;color:#16232E;"><access_level></span></td>
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
        <a href="<accept_url>" style="display:block;padding:15px 30px;font-family:Arial,Helvetica,sans-serif;font-size:16px;font-weight:bold;line-height:20px;mso-line-height-rule:exactly;color:#FFFFFF;text-decoration:none;">Accept invite</a>
      </td>
      <td width="12" style="width:12px;">&nbsp;</td>
      <td align="center" bgcolor="#FFFFFF" style="background-color:#FFFFFF;border:1px solid #D7E0E8;border-radius:9px;">
        <a href="<decline_url>" style="display:block;padding:14px 24px;font-family:Arial,Helvetica,sans-serif;font-size:16px;font-weight:bold;line-height:20px;mso-line-height-rule:exactly;color:#16232E;text-decoration:none;">Decline</a>
      </td>
    </tr></table>
  </td></tr>

  <tr><td class="pad" style="padding:18px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:21px;mso-line-height-rule:exactly;color:#7C8FA0;">
    The invite expires in 7 days. If the button doesn't work, copy this address:<br>
    <a href="<accept_url>" style="color:#0B7A6A;text-decoration:underline;word-break:break-all;"><accept_url></a>
  </td></tr>

  <tr><td class="pad" style="padding:22px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;border-top:1px solid #EEF2F6;"><tr><td height="1" style="height:1px;line-height:1px;font-size:1px;">&nbsp;</td></tr></table>
  </td></tr>

  <tr><td class="pad" style="padding:18px 40px 32px 40px;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:21px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Don't know <inviter_name>? Ignore this email — without your confirmation, no one gets access to anything of yours.
  </td></tr>

</table>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;">
  <tr><td class="pad" align="center" style="padding:20px 40px 8px 40px;font-family:Arial,Helvetica,sans-serif;font-size:12px;line-height:19px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Berry Finanças Ltda. &middot; Rua Exemplo, 123 &middot; São Paulo/SP &middot; 01000-000<br>
    <a href="https://berry.finance/suporte" style="color:#7C8FA0;text-decoration:underline;">Support</a> &nbsp;&middot;&nbsp;
    <a href="https://berry.finance/privacidade" style="color:#7C8FA0;text-decoration:underline;">Privacy</a>
  </td></tr>
</table>

</td></tr>
</table>
</body>
</html>
""""",
                        "en",
                        "wellcome",
                        "You've been invited to a Berry account",
                        1,
                        true,
                        TemplateCreatedAt,
                        AccountInviteRuleId
                    },
                    {
                        AccountInviteEsTemplateId,
                        """""
<!DOCTYPE html>
<html lang="es">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<meta http-equiv="X-UA-Compatible" content="IE=edge">
<meta name="color-scheme" content="light dark">
<meta name="supported-color-schemes" content="light dark">
<title>Has sido invitado a una cuenta de Berry</title>
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
<span style="display:none;font-size:1px;color:#F3F6F9;line-height:1px;max-height:0;max-width:0;opacity:0;overflow:hidden;"><inviter_name> te invitó a la cuenta "<account_name>" en Berry. La invitación expira en 7 días.</span>

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
    <inviter_name> te invitó a cuidar las cuentas juntos
  </td></tr>

  <tr><td class="pad" style="padding:14px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:25px;mso-line-height-rule:exactly;color:#54697A;">
    Al aceptar, podrás ver y registrar movimientos en la cuenta compartida — con el mismo saldo, las mismas categorías y la misma previsión de fin de mes.
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
                <td width="50%" style="width:50%;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:20px;mso-line-height-rule:exactly;color:#7C8FA0;">Cuenta<br><span style="font-size:15px;font-weight:bold;color:#16232E;"><account_name></span></td>
                <td width="50%" style="width:50%;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:20px;mso-line-height-rule:exactly;color:#7C8FA0;">Tu acceso<br><span style="font-size:15px;font-weight:bold;color:#16232E;"><access_level></span></td>
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
        <a href="<accept_url>" style="display:block;padding:15px 30px;font-family:Arial,Helvetica,sans-serif;font-size:16px;font-weight:bold;line-height:20px;mso-line-height-rule:exactly;color:#FFFFFF;text-decoration:none;">Aceptar invitación</a>
      </td>
      <td width="12" style="width:12px;">&nbsp;</td>
      <td align="center" bgcolor="#FFFFFF" style="background-color:#FFFFFF;border:1px solid #D7E0E8;border-radius:9px;">
        <a href="<decline_url>" style="display:block;padding:14px 24px;font-family:Arial,Helvetica,sans-serif;font-size:16px;font-weight:bold;line-height:20px;mso-line-height-rule:exactly;color:#16232E;text-decoration:none;">Rechazar</a>
      </td>
    </tr></table>
  </td></tr>

  <tr><td class="pad" style="padding:18px 40px 0 40px;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:21px;mso-line-height-rule:exactly;color:#7C8FA0;">
    La invitación expira en 7 días. Si el botón no funciona, copia esta dirección:<br>
    <a href="<accept_url>" style="color:#0B7A6A;text-decoration:underline;word-break:break-all;"><accept_url></a>
  </td></tr>

  <tr><td class="pad" style="padding:22px 40px 0 40px;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="width:100%;border-top:1px solid #EEF2F6;"><tr><td height="1" style="height:1px;line-height:1px;font-size:1px;">&nbsp;</td></tr></table>
  </td></tr>

  <tr><td class="pad" style="padding:18px 40px 32px 40px;font-family:Arial,Helvetica,sans-serif;font-size:13px;line-height:21px;mso-line-height-rule:exactly;color:#7C8FA0;">
    ¿No conoces a <inviter_name>? Ignora este correo — sin tu confirmación, nadie tendrá acceso a nada tuyo.
  </td></tr>

</table>

<table role="presentation" cellpadding="0" cellspacing="0" border="0" width="600" class="wrap" style="width:600px;max-width:600px;">
  <tr><td class="pad" align="center" style="padding:20px 40px 8px 40px;font-family:Arial,Helvetica,sans-serif;font-size:12px;line-height:19px;mso-line-height-rule:exactly;color:#7C8FA0;">
    Berry Finanças Ltda. &middot; Rua Exemplo, 123 &middot; São Paulo/SP &middot; 01000-000<br>
    <a href="https://berry.finance/suporte" style="color:#7C8FA0;text-decoration:underline;">Soporte</a> &nbsp;&middot;&nbsp;
    <a href="https://berry.finance/privacidade" style="color:#7C8FA0;text-decoration:underline;">Privacidad</a>
  </td></tr>
</table>

</td></tr>
</table>
</body>
</html>
""""",
                        "es",
                        "wellcome",
                        "Has sido invitado a una cuenta de Berry",
                        1,
                        true,
                        TemplateCreatedAt,
                        AccountInviteRuleId
                    },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Communication",
                table: "Template",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    OnboardingVerificationCodeEnTemplateId,
                    OnboardingVerificationCodeEsTemplateId,
                    WelcomeAfterVerificationEnTemplateId,
                    WelcomeAfterVerificationEsTemplateId,
                    AccountInviteEnTemplateId,
                    AccountInviteEsTemplateId,
                });

            migrationBuilder.UpdateData(
                schema: "Communication",
                table: "Template",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    OnboardingVerificationCodeTemplateId,
                    WelcomeAfterVerificationTemplateId,
                    AccountInviteTemplateId,
                },
                column: "ContentLanguage",
                values: new object[] { "en", "en", "en" });
        }
    }
}
