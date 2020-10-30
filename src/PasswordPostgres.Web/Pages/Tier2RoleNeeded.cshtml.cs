using Microsoft.AspNetCore.Mvc.RazorPages;
using static PasswordPostgres.Web.CDRole;

namespace PasswordPostgres.Web.Pages
{
    [AuthorizeRoles(Tier2, Admin)]
    public class Tier2RoleNeeded : PageModel
    {
        public void OnGet()
        {
        }
    }
}
