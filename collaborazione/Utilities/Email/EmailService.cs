using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace collaborazione.Utilities.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(string fromDisplayName, string fromEmailAddress, string toName, string toEmailAddress, string subject, string message)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(fromDisplayName, fromEmailAddress));
                email.To.Add(new MailboxAddress(toName, toEmailAddress));
                email.Subject = subject;

                var body = new BodyBuilder
                {
                    HtmlBody = @message
                };

                email.Body = body.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    //client.ServerCertificateValidationCallback = (sender, certificate, certChainType, error) => true;
                    //client.AuthenticationMechanisms.Remove("XOAUTH2");

                    MailKit.Security.SecureSocketOptions secureSocketOptions = new MailKit.Security.SecureSocketOptions();

                    if (Convert.ToBoolean(_config["EmailHostAuto"].ToString()))
                    {
                        secureSocketOptions = MailKit.Security.SecureSocketOptions.Auto;
                    }
                    if (Convert.ToBoolean(_config["EmailHostNone"].ToString()))
                    {
                        secureSocketOptions = MailKit.Security.SecureSocketOptions.None;
                    }
                    if (Convert.ToBoolean(_config["EmailHostSslOnConnect"].ToString()))
                    {
                        secureSocketOptions = MailKit.Security.SecureSocketOptions.SslOnConnect;
                    }
                    if (Convert.ToBoolean(_config["EmailHostStartTls"].ToString()))
                    {
                        secureSocketOptions = MailKit.Security.SecureSocketOptions.StartTls;
                    }
                    if (Convert.ToBoolean(_config["EmailHostStartTlsWhenAvailable"].ToString()))
                    {
                        secureSocketOptions = MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable;
                    }

                    await client.ConnectAsync(_config["EmailHost"], Convert.ToInt32(_config["EmailHostPort"]), secureSocketOptions).ConfigureAwait(false);
                    await client.AuthenticateAsync(_config["EmailHostUserName"], _config["EmailHostPassword"]).ConfigureAwait(false);

                    await client.SendAsync(email).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);

                    return true;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
