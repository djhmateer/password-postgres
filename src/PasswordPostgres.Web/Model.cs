using Microsoft.AspNetCore.Authorization;

namespace PasswordPostgres.Web
{

    public class Login
    {
        public int LoginId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool Verified { get; set; }

        //public Result<Account, TardisFault> AssertAccount(Account account)
        //{
        //    return account.LoginId == LoginId
        //        ? (Result<Account, TardisFault>) account
        //        : new TardisFault(HttpStatusCode.NotFound, "Not Found");
        //}
    }



    public class Model
    {
        public string Email { get; set; }
        //public string FullName { get; set; }
        //public bool IsAdmin { get; set; }
        public string CDRole { get; set; }
    }

    public static class CDRole
    {
        public const string Tier1 = "Tier1";
        public const string Tier2 = "Tier2";
        public const string Admin = "Admin";
    }


    // https://stackoverflow.com/a/24182340/26086
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles) => Roles = string.Join(",", roles);
    }

    public class EmailMessage
    {
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
