using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AffärsLager.Services
{
    public class MailService
    {
        // SMTP-konfiguration med nya credentials
        private string _smtpHost = "smtp.gmail.com";
        private int _smtpPort = 587;
        private string _smtpUsername = "victorberg66@gmail.com";
        private string _smtpPassword = "rtci btcl twqc nzbx";
        private string _fromEmail = "victorberg66@gmail.com";
        private string _fromName = "RestoNation System";

        // Lagra senaste felmeddelandet
        public string? LastError { get; private set; }

        /// <summary>
        /// Konfigurerar SMTP-inställningar
        /// </summary>
        public void ConfigureSMTP(string host, int port, string username, string password, string fromEmail, string fromName)
        {
            _smtpHost = host;
            _smtpPort = port;
            _smtpUsername = username;
            _smtpPassword = password;
            _fromEmail = fromEmail;
            _fromName = fromName;
        }

        /// <summary>
        /// Skickar ett e-postmeddelande
        /// </summary>
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, string? plainTextBody = null)
        {
            try
            {
                LastError = null;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody,
                    TextBody = plainTextBody ?? htmlBody
                };

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                LastError = $"{ex.GetType().Name}: {ex.Message}";
                if (ex.InnerException != null)
                {
                    LastError += $"\nInner: {ex.InnerException.Message}";
                }
                return false;
            }
        }

        /// <summary>
        /// Skickar e-post med bilaga
        /// </summary>
        public async Task<bool> SendEmailWithAttachmentAsync(string toEmail, string subject, string htmlBody, string attachmentPath, string? plainTextBody = null)
        {
            try
            {
                LastError = null;

                // Validera att filen finns
                if (!File.Exists(attachmentPath))
                {
                    LastError = $"Bilagefilen hittades inte: {attachmentPath}";
                    return false;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody,
                    TextBody = plainTextBody ?? htmlBody
                };

                // Lägg till bilaga
                bodyBuilder.Attachments.Add(attachmentPath);

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                LastError = $"{ex.GetType().Name}: {ex.Message}";
                if (ex.InnerException != null)
                {
                    LastError += $"\nInner: {ex.InnerException.Message}";
                }
                return false;
            }
        }

        /// <summary>
        /// Skickar statistikrapport via e-post
        /// </summary>
        public async Task<bool> SendStatistikRapportAsync(string toEmail, string restaurangNamn, string period, string pdfPath)
        {
            string subject = $"RestoNation - Statistikrapport {restaurangNamn} ({period})";
            string htmlBody = $@"
                <html>
                <body>
                    <h2>Statistikrapport - {restaurangNamn}</h2>
                    <p>Period: {period}</p>
                    <p>Bifogad finner du statistikrapport i PDF-format.</p>
                    <br>
                    <p>Med vänliga hälsningar,<br>RestoNation System</p>
                </body>
                </html>";

            return await SendEmailWithAttachmentAsync(toEmail, subject, htmlBody, pdfPath);
        }

        /// <summary>
        /// Skickar bokföringsfil via e-post
        /// </summary>
        public async Task<bool> SendBokforingsfilAsync(string toEmail, DateTime datum, string filePath)
        {
            string subject = $"RestoNation - Bokföringsfil {datum:yyyy-MM-dd}";
            string htmlBody = $@"
                <html>
                <body>
                    <h2>Bokföringsfil - Dagsavslut</h2>
                    <p>Datum: {datum:yyyy-MM-dd}</p>
                    <p>Bifogad finner du bokföringsfilen med alla transaktioner från dagens försäljning.</p>
                    <br>
                    <p>Med vänliga hälsningar,<br>RestoNation System</p>
                </body>
                </html>";

            return await SendEmailWithAttachmentAsync(toEmail, subject, htmlBody, filePath);
        }
    }
}