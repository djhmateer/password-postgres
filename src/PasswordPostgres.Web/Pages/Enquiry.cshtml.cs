using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

            var filepath = Directory.GetCurrentDirectory();
            Log.Information($"filepath from Directory.GetCurrentDirectory() is {filepath}");

            string apiKey;
            bool isLinux = false;
            if (filepath == "/var/www/web")
            {
                Log.Information("Linux looking for apikey for sendgrid");
                // https://stackoverflow.com/a/15259355/26086
                var thing = await System.IO.File.ReadAllTextAsync(filepath + "/secrets/sendgrid-passwordpostgres.txt");

                for (int ctr = 0; ctr < thing.Length; ctr++)
                {
                    if (char.IsControl(thing, ctr))
                        Log.Information("Control character \\U{0} found in position {1}.",
                            Convert.ToInt32(thing[ctr]).ToString("X4"), ctr);
                } 
                
                apiKey = new string(thing.Where(c => !char.IsControl(c)).ToArray());
                isLinux = true;
            }
            else
            {
                Log.Information("Windows looking for apikey for sendgrid");
                apiKey = await System.IO.File.ReadAllTextAsync("../../secrets/sendgrid-passwordpostgres.txt");
            }

            Log.Information($"API key is {apiKey}");

            var client = new SendGridClient(apiKey);
            var time = DateTime.Now.ToString("HH:mm:ss");
            var msg = new SendGridMessage
            {
                From = new EmailAddress("test@example.com", "DX Team"),
                Subject = $"PasswordPostgres linux {isLinux} time sent is {time}",
                PlainTextContent = "Hello, Email!",
                HtmlContent = "<strong>Hello, Email!</strong>"
            };
            //msg.AddTo(new EmailAddress("davemateer@mailinator.com", "Test User"));
            msg.AddTo(new EmailAddress("davemateer@mailinator.com"));

            msg.AddContent(MimeType.Text, "Hello World plain text!");
            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                // have retry logic
                Log.Information($"response is {response.Headers}");
                ModelState.AddModelError(string.Empty, $"Problem sending email - status code is {response.StatusCode}");
                return Page();
            }

            return RedirectToPage("EnquirySent");
        }
    }
}
