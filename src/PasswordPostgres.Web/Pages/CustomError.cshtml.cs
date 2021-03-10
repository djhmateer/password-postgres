using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace PasswordPostgres.Web.Pages
{
    // todo - dig into these attributes
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-5.0
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class CustomErrorModel : PageModel
    {
        public int? CustomStatusCode { get; set; }

        public void OnGet(int? statusCode = null)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            // a non 500 eg 404
            // this can be non page requests eg /js/site-chart.js
            // feature can be null when a 500 is thrown
            if (feature != null)
            {

                //Log.Warning($"Http Status code {statusCode} on {feature.OriginalPath}");
                CustomStatusCode = statusCode;
                return;
            }

            // a 500
            // relying on serilog to output the error
            //var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            // integration tests can call a page where the exceptionHandlerPathFeature can be null
            CustomStatusCode = 500;

            // somewhere else is emitting the Log.Error stacktracke
            //Log.Error($"Exception is {exceptionHandlerPathFeature.Error}");

            //OriginalPath = exceptionHandlerPathFeature.Path;
            //Exception exception = exceptionHandlerPathFeature.Error;
        }

        //public ActionResult OnPost()
        public void OnPost()
        {
            Log.Warning( "ASP.NET failure - maybe antiforgery. Caught by OnPost Custom Error. Sending a 400 to the user which is probable");
            Log.Warning("Need to take off minimumlevel override in Program.cs for more information");
            CustomStatusCode = 400;

            //return Page();
        }
    }
}
