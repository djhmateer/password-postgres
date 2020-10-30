using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace PasswordPostgres.Web.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly string connectionString;
        public RegisterModel(IConfiguration c) => connectionString = c.GetConnectionString("Default");

        [BindProperty] public InputModel? Input { get; set; }
        public string? ReturnUrl { get; set; }
        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = null!; // can never be null

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = null!; // can never be null

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string? ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // check if the emailaddress exists in db?
                var result = await Db.LoginByEmail(connectionString, Input.Email);

                if (result != null)
                {
                    Log.Information($@"User tried to register an already registered email address {Input.Email}");
                    ModelState.AddModelError(string.Empty, "Sorry email address is already registered - try logging in or resetting password");
                    return Page();
                }

                var login = new Login
                {
                    Email = Input.Email,
                    PasswordHash = Input.Password.HashPassword(),
                    Verified = true
                };

                var returnedLogin = await Db.InsertLogin(connectionString, login);

                // generate email confirmation with a token (

                // do I actually want to sign them in?

                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                //var callbackUrl = Url.Page(
                //    "/Account/ConfirmEmail",
                //    pageHandler: null,
                //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                //    protocol: Request.Scheme);

                //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                //await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl ?? "/");

                //foreach (var error in result.Errors)
                //{
                //    ModelState.AddModelError(string.Empty, error.Description);
                //}

                //// If we got this far, something failed, redisplay form
                //return Page();
            }

            return Page();
        }
    }
}
