using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordPostgres.Web;
using static PasswordPostgres.Web.CDRole;

namespace PasswordPostgres.Web.Pages
{
    [AuthorizeRoles(Tier1, Tier2, Admin)]
    public class CrawlModel : PageModel
    {
        public string? Message { get; set; }

        public void OnGet()
        {
            var roleClaims = User.FindAll(ClaimTypes.Role);

            Message = "Role claims are: ";
            foreach (var claim in roleClaims)
            {
                // Tier1, Tier2, Admin etc...
                Message += claim.Value + " ";
            }
        }
    }
}
