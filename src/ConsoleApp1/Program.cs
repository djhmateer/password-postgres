using System;
using System.IO;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main()
        {
            //var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");

            var filepath = Directory.GetCurrentDirectory();
            string apiKey;
            if (filepath == "/var/www/web")
            {
                apiKey = await File.ReadAllTextAsync(filepath + "/secrets/sendgrid-passwordpostgres.txt");
            }
            else
            {
                //apiKey = await File.ReadAllTextAsync("../../secrets/sendgrid-passwordpostgres.txt");
                apiKey = await File.ReadAllTextAsync("../../../../../secrets/sendgrid-passwordpostgres.txt");
            }
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("test@example.com", "DX Team"),
                Subject = "Sending with Twilio SendGrid is Fun",
                PlainTextContent = "and easy to do anywhere, even with C#",
                HtmlContent = "<strong>and easy to do anywhere, even with C#</strong>"
            };
            msg.AddTo(new EmailAddress("test@example.com", "Test User"));
            var response = await client.SendEmailAsync(msg);

            Console.WriteLine($"status is: {response.StatusCode}");
        }
    }
}
