using Alfa_1.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Alfa_1.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {

        public SmtpConfig SmtpConfig { get; }
        public async Task SendEmailAsync(string email, string subject, string message)
        {

            // Plug in your email service here to send an email.
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(subject, "ipgfix@hotmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            await Task.Run(() =>
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.live.com", 587, false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("ipgfix@hotmail.com", "ipg-1007249");

                    client.Send(emailMessage);
                    client.Disconnect(true);
                }
            });


        }


        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
