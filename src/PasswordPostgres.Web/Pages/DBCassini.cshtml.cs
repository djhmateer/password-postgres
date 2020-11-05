using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PasswordPostgres.Web.Pages
{
    public class DBCassiniModel : PageModel
    {
        public IList<Thing>? Things { get; set; }

        public async Task OnGetAsync()
        {
            var connectionString = AppConfiguration.LoadConnectionStringFromEnvironment().ConnectionString;

            var things = await Db.GetThings(connectionString);
            Things = things.ToList();
        }
    }

    public class Thing
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Team { get; set; }
        public string Target { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
