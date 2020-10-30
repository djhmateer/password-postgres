using System.Threading.Tasks;

namespace PasswordPostgres.Web
{
    public interface IEmailService
    {
        Task SendAsync(string sender, string recipient, string title, string content);
    }
}
