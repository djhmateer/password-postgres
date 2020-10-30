using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PasswordPostgres.Web.Pages
{
    public class HostingEnvironmentModel : PageModel
    {
        public string? EnvironmentString { get; set; }

        public async Task OnGetAsync()
        {
            EnvironmentString = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }
    }
}
