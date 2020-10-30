using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PasswordPostgres.Web.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorModel : PageModel
    {
        public string? Message { get; set; }
        public string? OriginalPath { get; set; }

        public void OnGet()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            OriginalPath = exceptionHandlerPathFeature.Path;
            Exception exception = exceptionHandlerPathFeature.Error;

            Message = "Message: " + exceptionHandlerPathFeature.Error.Message;
            Message += ", InnerException.Message: " + exceptionHandlerPathFeature.Error.InnerException?.Message;
            Message += ", Type: " + exception.GetType();
        }
    }
}
