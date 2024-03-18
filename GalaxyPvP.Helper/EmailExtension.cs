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
        static string htmlContentTemplate = "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Email Template</title>\r\n    <style>\r\n        body {\r\n            font-family: 'Arial', sans-serif;\r\n            margin: 0;\r\n            padding: 0;\r\n            background-color: #f4f4f4;\r\n        }\r\n\r\n        .container {\r\n            width: 100%;\r\n            max-width: 600px;\r\n            margin: 0 auto;\r\n            background-color: #ffffff;\r\n            padding: 20px;\r\n            border-radius: 5px;\r\n            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);\r\n        }\r\n\r\n        h1 {\r\n            color: #333333;\r\n        }\r\n\r\n        p {\r\n            color: #555555;\r\n            font-weight:500;\r\n        }\r\n\r\n        .message-container {\r\n            display: flex;\r\n            justify-content: center;\r\n            \r\n        }\r\n\r\n        .message {\r\n            text-align: center;\r\n            background: #211C6A;\r\n            width: 400px;\r\n            height: 30px;\r\n            line-height: 30px; /* Center text vertically */\r\n            color: white;\r\n            border-radius: 5px;\r\n            margin: 0 auto; /* Center the container */\r\n        }\r\n\r\n            .message p {\r\n                font-size: large;\r\n                font-weight: bolder;\r\n                color: white;\r\n                margin: 0; /* Remove default paragraph margin */\r\n            }\r\n\r\n        /*.message {\r\n            text-align: center;\r\n            background: #211C6A;\r\n            width: 400px;\r\n            height: 30px;\r\n            display: flex;\r\n            align-items: center;\r\n            justify-content: center;\r\n            color: white;\r\n            border-radius: 5px;\r\n        }\r\n\r\n            .message p {\r\n                text-align: center;\r\n                font-size: large;\r\n                font-weight: bolder;\r\n                color: white;\r\n            }*/\r\n\r\n        img {\r\n            width: 100%;\r\n            height: auto;\r\n            margin: auto;\r\n            text-align: center;\r\n            background-color: #1D2B53;\r\n        }\r\n\r\n        .image {\r\n            display: flex;\r\n            justify-content: center;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <h1>{EMAIL_SUBJECT}</h1>\r\n        <div class=\"image\">\r\n            <img src=\"https://gfcplayfabcloud.blob.core.windows.net/gfc-storage/GFC%20fighter%20logo.png\" alt=\"Email Image\" />\r\n        </div>\r\n        <p>Hello there!</p>\r\n        <!--<p>{BODY_CONTENT}</p>-->\r\n        <p>Please back to your game and use this code to reset your password.</p><br />\r\n        <!--<div class=\"message-container\" style=\"justify-content:center\">\r\n        <div class=\"message\" style=\"justify-content:center; align-items:center;\">\r\n            <p>{BODY_CONTENT}</p>\r\n        </div>\r\n    </div>-->\r\n        <div class=\"message\">\r\n            <p>{BODY_CONTENT}</p>\r\n        </div>\r\n        <p>Best regards,<br>Galaxy Team</p>\r\n    </div>\r\n</body>\r\n</html>";
        public static async Task SendGridEmailAsync(string recipient, string subject, string body)
        {
            // Replace the placeholder with the actual body content
            string finalHtmlContent = htmlContentTemplate
                        .Replace("{EMAIL_SUBJECT}", subject)
                        .Replace("{BODY_CONTENT}", body);


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
                //HtmlContent = GetEmailHtmlContent(subject, body),
                HtmlContent = finalHtmlContent,
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

        private static string GetEmailHtmlContent(string subject, string body)
        {
            // Get the current directory of the application
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Combine the current directory with the relative path to the template
            string templatePath = Path.Combine(currentDirectory, "emailTemplate.html");

            string htmlTemplate = templatePath;
            return htmlTemplate.Replace("{subject}", subject).Replace("{body}", body);
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
