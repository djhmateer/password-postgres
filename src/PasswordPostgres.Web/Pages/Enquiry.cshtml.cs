using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
//using SendGrid;
//using SendGrid.Helpers.Mail;
using Serilog;
using PostmarkDotNet;
using PostmarkDotNet.Model;

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
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid) return Page();

            var filepath = Directory.GetCurrentDirectory();

            string apiKey;
            bool isLinux = false;
            if (filepath == "/var/www/web")
            {
                Log.Information("Linux looking for apikey for postmark");
                apiKey = await System.IO.File.ReadAllTextAsync(filepath + "/secrets/postmark-passwordpostgres.txt");
                isLinux = true;
            }
            else
            {
                Log.Information("Windows looking for apikey for postmark");
                apiKey = await System.IO.File.ReadAllTextAsync("../../secrets/postmark-passwordpostgres.txt");
            }

            var time = DateTime.Now.ToString("HH:mm:ss");

            var message = new PostmarkMessage()
            {
                //To = "davemateer@mailinator.com",
                //To = "pen@hmsoftware.co.uk",
                To = "dave@hmsoftware.co.uk",
                From = "dave@hmsoftware.co.uk",
                //TrackOpens = true,
                Subject = $"A complex email {time}",
                TextBody = "Plain Text Body - hello world",
                //HtmlBody = "<html><body><img src=\"cid:embed_name.jpg\"/></body></html>",
                HtmlBody = "<html><body><p>Hello world</p></body></html>",
                //Tag = "business-message",
                //Headers = new HeaderCollection{
                //    {"X-CUSTOM-HEADER", "Header content"}
                //}
            };

            //var imageContent = System.IO.File.ReadAllBytes("test.jpg");
            //message.AddAttachment(imageContent, "test.jpg", "image/jpg", "cid:embed_name.jpg");

            var serverToken = apiKey;
            var client = new PostmarkClient(serverToken);
            try
            {
                var sendResult = await client.SendMessageAsync(message);

                // If there's a problem with the content of your message,
                // the API will still return, but with an error status code, 
                // you should take appropriate steps to resolve/retry if this 
                // happens.
                if (sendResult.Status == PostmarkStatus.Success)
                {
                    /* Handle success */
                    Log.Information("send success");
                }
                else
                {
                    Log.Warning($"send fail Postmark {sendResult.Status} {sendResult.Message}");
                    ModelState.AddModelError(string.Empty, $"Problem sending email - status code is {sendResult.Status} and message {sendResult.Message}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                // Calls to the client can throw an exception 
                // if the request to the API times out.
                Log.Error($"Sending mail via Postmark error {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Exception sending message - timeout? {ex.Message}");
                return Page();

            }
            //var msg = new SendGridMessage
            //{
            //    From = new EmailAddress("test@example.com", "DX Team"),
            //    Subject = $"PasswordPostgres linux {isLinux} time sent is {time}",
            //    PlainTextContent = "Hello, Email!",
            //    HtmlContent = "<strong>Hello, Email!</strong>"
            //};
            ////msg.AddTo(new EmailAddress("davemateer@mailinator.com", "Test User"));
            //msg.AddTo(new EmailAddress("davemateer@mailinator.com"));

            //var response = await client.SendEmailAsync(msg);

            //if (response.StatusCode != HttpStatusCode.Accepted)
            //{
            //    // need retry logic
            //    ModelState.AddModelError(string.Empty, $"Problem sending email - status code is {response.StatusCode}");
            //    return Page();
            //}

            return RedirectToPage("EnquirySent");
        }


        // https://github.com/sendgrid/sendgrid-csharp#hello
        // https://github.com/sendgrid/sendgrid-csharp/blob/main/USE_CASES.md
        //public async Task<IActionResult> OnPostAsyncSendGrid()
        //{
        //    //if (!ModelState.IsValid) return Page();

        //    var filepath = Directory.GetCurrentDirectory();
        //    Log.Information($"filepath from Directory.GetCurrentDirectory() is {filepath}");

        //    string apiKey;
        //    bool isLinux = false;
        //    if (filepath == "/var/www/web")
        //    {
        //        Log.Information("Linux looking for apikey for sendgrid");
        //        // https://stackoverflow.com/a/15259355/26086
        //        apiKey = await System.IO.File.ReadAllTextAsync(filepath + "/secrets/sendgrid-passwordpostgres.txt");
        //        isLinux = true;
        //    }
        //    else
        //    {
        //        Log.Information("Windows looking for apikey for sendgrid");
        //        apiKey = await System.IO.File.ReadAllTextAsync("../../secrets/sendgrid-passwordpostgres.txt");
        //    }

        //    var client = new SendGridClient(apiKey);
        //    var time = DateTime.Now.ToString("HH:mm:ss");
        //    var msg = new SendGridMessage
        //    {
        //        From = new EmailAddress("test@example.com", "DX Team"),
        //        Subject = $"PasswordPostgres linux {isLinux} time sent is {time}",
        //        PlainTextContent = "Hello, Email!",
        //        HtmlContent = "<strong>Hello, Email!</strong>"
        //    };
        //    //msg.AddTo(new EmailAddress("davemateer@mailinator.com", "Test User"));
        //    msg.AddTo(new EmailAddress("davemateer@mailinator.com"));

        //    var response = await client.SendEmailAsync(msg);

        //    if (response.StatusCode != HttpStatusCode.Accepted)
        //    {
        //        // need retry logic
        //        ModelState.AddModelError(string.Empty, $"Problem sending email - status code is {response.StatusCode}");
        //        return Page();
        //    }

        //    return RedirectToPage("EnquirySent");
        //}
    }
}
