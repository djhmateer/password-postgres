using Microsoft.AspNetCore.Mvc.RazorPages;
using static PasswordPostgres.Web.CDRole;

namespace PasswordPostgres.Web.Pages
{
    [AuthorizeRoles(Tier1, Tier2, Admin)]
    public class Tier1RoleNeeded : PageModel
    {
        public void OnGet()
        {
        }
    }
}
