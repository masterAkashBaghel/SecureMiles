using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SecureMiles.Services.Mail
{
    public class SmtpSettings
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public class EmailService(IConfiguration configuration)
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        private readonly SmtpSettings _smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
#pragma warning restore CS8601 // Possible null reference assignment.

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password),
                    EnableSsl = true
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.UserName!),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage).ConfigureAwait(false);
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.Message}");
                throw new InvalidOperationException("Failed to send email.", smtpEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                throw new InvalidOperationException("An unexpected error occurred while sending email.", ex);
            }
        }


        // This method is used to send an email with an attachment

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachmentData, string attachmentName)
        {
            try
            {
                using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password),
                    EnableSsl = true
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.UserName!),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                if (attachmentData != null && attachmentData.Length > 0)
                {
                    var attachmentStream = new MemoryStream(attachmentData);
                    var attachment = new Attachment(attachmentStream, attachmentName);
                    mailMessage.Attachments.Add(attachment);
                }
                else
                {
                    throw new ArgumentException("Attachment data is null or empty.", nameof(attachmentData));
                }

                await client.SendMailAsync(mailMessage).ConfigureAwait(false);
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.Message}");
                throw new InvalidOperationException("Failed to send email.", smtpEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                throw new InvalidOperationException("An unexpected error occurred while sending email.", ex);
            }
        }
    }
}