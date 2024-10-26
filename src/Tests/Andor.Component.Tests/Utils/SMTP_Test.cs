using Andor.Application.Communications.Interfaces;

namespace Andor.Component.Tests.Utils
{
    internal class SMTP_Test : ISMTP
    {
        public static List<Mail> Emails()
        {
            if (_email is null)
            {
                _email = new List<Mail>();
            }

            return _email;
        }

        private static List<Mail> _email;

        public static SMTP_Test Instance()
        {
            if (sMTP_Test is null)
            {
                sMTP_Test = new SMTP_Test();
            }

            return sMTP_Test;
        }

        private static SMTP_Test sMTP_Test;

        private SMTP_Test()
        {

        }

        public Task Handler(string recipientMail, string body, string Subject, CancellationToken cancellationToken)
        {
            _ = Emails();
            _email.Add(new Mail(recipientMail, body, Subject));

            return Task.CompletedTask;
        }
    }

    internal record Mail(string recipientMail, string body, string Subject);

}
