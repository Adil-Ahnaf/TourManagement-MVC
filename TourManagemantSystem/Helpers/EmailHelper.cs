using Magnum.FileSystem;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace TourManagemantSystem.Helpers
{
    public class EmailHelper : IEmailHelper
    {
        private readonly IOptions<SmtpSetting> smtpSetting;

        public EmailHelper(IOptions<SmtpSetting> smtpSetting)
        {
            this.smtpSetting = smtpSetting;
        }

        public async Task SendAsync(string from, string to, string subject, string body, string filePath)
        {
            var message = new MailMessage(from,
                    to,
                    subject,
                    body
                    );
            message.IsBodyHtml = true;
            if (!string.IsNullOrEmpty(filePath))
            {
                var attaachment = new Attachment(filePath);
                message.Attachments.Add(attaachment);
            }

            using (var emailClient = new SmtpClient(smtpSetting.Value.Host, smtpSetting.Value.Port))
            {
                emailClient.EnableSsl = true;
                emailClient.Credentials = new NetworkCredential(
                    smtpSetting.Value.User,
                    smtpSetting.Value.Password);

                await emailClient.SendMailAsync(message);
            }
        }
    }
}
