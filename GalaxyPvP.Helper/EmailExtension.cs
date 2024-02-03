using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGrid.Helpers.Mail.Model;

namespace GalaxyPvP.Extensions
{
    public class EmailExtension
    {
        public static string SendGridEmail;
        public static string SendGridKey;
        public static string SendGridPass;
        public static async Task SendGridEmailAsync(string recipient, string subject, string body)
        {
            

            if (string.IsNullOrEmpty(SendGridEmail) || string.IsNullOrEmpty(SendGridPass) || string.IsNullOrEmpty(SendGridKey))
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("sendGridSetting.json")
                    .Build();
                SendGridEmail = configuration["SendGridEmail"];
                SendGridPass = configuration["SendGridPass"];
                SendGridKey = configuration["SendGridApiKey"];
            }

            var client = new SendGridClient(SendGridKey);

            var msg = new SendGridMessage
            {
                From = new EmailAddress(SendGridEmail, "Galaxy"),
                Subject = subject,
                PlainTextContent = "Plain text content of the email",
                HtmlContent = body,
            };

            var to = new EmailAddress(recipient);
            msg.AddTo(to);

            try
            {
                var response = client.SendEmailAsync(msg).Result;
                Console.WriteLine($"Status Code: {response.StatusCode}");
                Console.WriteLine($"Response Body: {await response.Body.ReadAsStringAsync()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static async Task<bool> SendEmailAsync(string recipient, string subject, string body)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(SendGridEmail, SendGridPass);

                var message = new MailMessage
                {
                    From = new MailAddress(SendGridEmail),
                    To = { recipient },
                    Subject = subject,
                    Body = body
                };
                message.BodyEncoding = Encoding.UTF8;
                message.SubjectEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                message.ReplyToList.Add(new MailAddress(SendGridEmail));
                message.Sender = new MailAddress(SendGridEmail);

                try
                {
                    await client.SendMailAsync(message);
                    return true;
                }
                catch (SmtpException ex)
                {
                    // Lỗi xảy ra khi gửi email
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            }
        }
    }
}
