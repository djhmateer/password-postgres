using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordPostgres.Web;
using static PasswordPostgres.Web.CDRole;

namespace PasswordPostgres.Web.Pages
{
    [AuthorizeRoles(Admin)]
    public class AdminRoleNeededModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
