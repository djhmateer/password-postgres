using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace PasswordPostgres.Web.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = null!;

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = null!;
        }

        private readonly string connectionString;
        public ForgotPasswordModel(IConfiguration c) => connectionString = c.GetConnectionString("Default");

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var login = await Db.LoginByEmail(connectionString, Input.Email);

                if (login == null)
                {
                    Log.Information("An email not found in our db was entered, but we won't tell the user that");
                    //ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }


                //// For more information on how to enable account confirmation and password reset please 
                //// visit https://go.microsoft.com/fwlink/?LinkID=532713
                //var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                //var callbackUrl = Url.Page(
                //    "/Account/ResetPassword",
                //    pageHandler: null,
                //    values: new { area = "Identity", code },
                //    protocol: Request.Scheme);

                //await _emailSender.SendEmailAsync(
                //    Input.Email,
                //    "Reset Password",
                //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }

    }
}
