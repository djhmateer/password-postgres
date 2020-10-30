using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PasswordPostgres.Web.Pages
{
    public class ThrowExceptionModel : PageModel
    {
        public async Task OnGetAsync()
        {
            await Task.Delay(100);
            throw new ApplicationException("Some logic went wrong - maybe an id not found that was expected");
        }
    }
}
