using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PasswordPostgres.Web.Pages
{
    public class DBTestModel : PageModel
    {
        public string? ConnectionString { get; set; }
        public IList<Employee>? Employees { get; set; }

        public async Task OnGetAsync()
        {
            var connectionString = AppConfiguration.LoadConnectionStringFromEnvironment().ConnectionString;
            ConnectionString = connectionString;

            var employees = await Db.GetEmployees(connectionString);
            Employees = employees.ToList();
        }
    }

   
    public class Employee
    {
        // hacking EF
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
    }
}
