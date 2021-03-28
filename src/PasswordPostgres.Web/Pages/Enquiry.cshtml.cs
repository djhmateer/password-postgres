using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PostmarkDotNet;
using Serilog;

namespace PasswordPostgres.Web.Pages
{
    public class EnquiryModel : PageModel
    {
        public EnquiryModel() => Message = "This is a test message";

        [Required]
        [EmailAddress]
        [BindProperty]
        public string? EmailAddress { get; set; }

        [Required]
        [BindProperty]
        public string? Subject { get; set; }

        [Required]
        [BindProperty]
        public string? Message { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            // Javascript should catch any errors, but just in case
            if (!ModelState.IsValid) return Page();


            var postmarkMessage = new PostmarkMessage
            {
                To = EmailAddress,
                From = "dave@hmsoftware.co.uk", // has to be a Sender Signature on postmark account
                //TrackOpens = true,
                Subject = Subject,
                //Subject = $"A complex email {time}",
                TextBody = Message,
                //TextBody = "Plain Text Body - hello world",
                //HtmlBody = "<html><body><img src=\"cid:embed_name.jpg\"/></body></html>",
                HtmlBody = "<html><body><p>Hello world</p></body></html>",
                //Tag = "business-message",
                //Headers = new HeaderCollection{
                //    {"X-CUSTOM-HEADER", "Header content"}
                //}
            };
            //var imageContent = System.IO.File.ReadAllBytes("test.jpg");
            //message.AddAttachment(imageContent, "test.jpg", "image/jpg", "cid:embed_name.jpg");

            var postmarkServerToken = AppConfiguration.LoadFromEnvironment().PostmarkServerToken;

            try
            {
                var sendResult = await Email.Send(postmarkServerToken, postmarkMessage);
                if (sendResult != null && sendResult.Status == PostmarkStatus.Success)
                {
                    Log.Information("send success");
                    return RedirectToPage("EnquirySent");
                }

                Log.Warning($"send fail Postmark {sendResult?.Status} {sendResult.Message}");
                ModelState.AddModelError(string.Empty, $"Problem sending email - status code is {sendResult.Status} and message {sendResult.Message}");
                return Page();
            }
            catch (Exception ex)
            {
                // Calls to the client can throw an exception 
                // if the request to the API times out.
                // or if the From address is not a Sender Signature 
                Log.Error($"Sending mail via Postmark error {ex.Message}");
                ModelState.AddModelError(string.Empty, $"Exception sending message {ex.Message}");
                return Page();
            }
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
