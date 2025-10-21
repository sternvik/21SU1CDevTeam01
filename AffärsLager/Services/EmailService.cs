using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace AffärsLager.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com"; // Gmail SMTP
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "restonation@gmail.com"; // Uppdatera med rätt email
        private readonly string _smtpPassword = "ditt-lösenord"; // VIKTIGT: Använd Gmail App Password!

        public async Task<bool> SkickaEmail(string till, string ämne, string meddelande, List<string>? bilagor = null)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("RestoNation System", _smtpUser));
                email.To.Add(new MailboxAddress("", till));
                email.Subject = ämne;

                var body = new BodyBuilder { HtmlBody = meddelande };

                // Lägg till bilagor
                if (bilagor != null)
                {
                    foreach (var bilaga in bilagor)
                    {
                        if (File.Exists(bilaga))
                        {
                            body.Attachments.Add(bilaga);
                        }
                    }
                }

                email.Body = body.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpUser, _smtpPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email-fel: {ex.Message}");
                return false;
            }
        }
    }
}