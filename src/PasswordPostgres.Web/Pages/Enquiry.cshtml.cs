using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PasswordPostgres.Web;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;

namespace PasswordPostgres.Web.Pages
{
    public class EnquiryModel : PageModel
    {
        private readonly IEmailService _emailService;

        public EnquiryModel(IEmailService emailService)
        {
            _emailService = emailService;
        }

        //[Required]
        [EmailAddress]
        [BindProperty]
        public string? Email { get; set; }

        //[Required]
        [BindProperty]
        public string? Subject { get; set; }

        //[Required]
        [BindProperty]
        public string? Message { get; set; }

        public void OnGet()
        {
        }

        // https://github.com/sendgrid/sendgrid-csharp#hello
        // https://github.com/sendgrid/sendgrid-csharp/blob/main/USE_CASES.md
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid) return Page();
            //var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            //var sendgrid_api_key = System.IO.File.ReadAllText("sendgrid-passwordpostgres.txt");

            // Windows: C:\dev\test\password-postgres\src\PasswordPostgres.Web
            // Linux: /var/www/web
            var filepath = Directory.GetCurrentDirectory();
            Log.Information($"filepath from Directory.GetCurrentDirectory() is {filepath}");

            // windows: C:\dev\test\password-postgres\src\PasswordPostgres.Web\bin\Debug\netcoreapp3.1
            // linux: /var/www/web
            //var asdf = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //Log.Information($"reflection filepath is {asdf}");

            string apiKey;
            if (filepath == "/var/www/web")
            {
                // Linux in Prod
                Log.Information("Linux looking for apikey for sendgrid");
                // I'm sure this worked from 4790!
                apiKey = await System.IO.File.ReadAllTextAsync(filepath + "/secrets/sendgrid-passwordpostgres.txt");
            }
            else
            {
                // this works on Windows in Dev
                Log.Information("Windows looking for apikey for sendgrid");
                apiKey = await System.IO.File.ReadAllTextAsync("../../secrets/sendgrid-passwordpostgres.txt");
            }

            Log.Information($"API key is {apiKey}");

            var client = new SendGridClient(apiKey);
            var time = System.DateTime.Now.ToString("HH:mm:ss");
            var msg = new SendGridMessage
            {
                From = new EmailAddress("test@example.com", "DX Team"),
                Subject = $"PasswordPostgres time sent is {time}",
                PlainTextContent = "Hello, Email!",
                HtmlContent = "<strong>Hello, Email!</strong>"
            };
            //msg.AddTo(new EmailAddress("davemateer@mailinator.com", "Test User"));
            msg.AddTo(new EmailAddress("davemateer@mailinator.com"));
            var response = await client.SendEmailAsync(msg);

            //_emailService.SendAsync(Email, "admin@example.com", Subject, Message);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                // have retry logic
                ModelState.AddModelError(string.Empty, $"Problem sending email - status code is {response.StatusCode}");
                return Page();
            }

            return RedirectToPage("EnquirySent");
        }
    }
}
