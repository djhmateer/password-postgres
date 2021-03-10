using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PostmarkDotNet;
using Serilog;

namespace PasswordPostgres.Web.Pages
{
    public class StatusCode400 : PageModel
    {
        //public async Task OnGetAsync()
        public ActionResult OnGet()
        {
            return new StatusCodeResult(400);
        }
    }
}
