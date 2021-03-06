using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using PasswordPostgres.Web;
using Serilog;

namespace PasswordPostgres.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty] 
        public InputModel Input { get; set; } = null!;
        public string? ReturnUrl { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = null!;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = null!;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        private readonly string conn;
        public LoginModel(IConfiguration c) => conn = c.GetConnectionString("Default");

        public async Task OnGetAsync(string? returnUrl = null)
        {
            //Log.Information(returnUrl);
            //if (!string.IsNullOrEmpty(ErrorMessage))
            //{
            //    ModelState.AddModelError(string.Empty, ErrorMessage);
            //}

            // I want people to be able to come to the login screen and be automatically redirected to /Tier1RoleNeeded
            // Clear the existing external cookie
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // can we catch if already authenticated?
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Log.Information("Authenticated!");
                ViewData["Message"] = "You are signed in already! Authenticated so redirect to /Tier1RoleNeeded";
            }
            else if (User.Identity == null)
            {
                //await HttpContext.AuthenticateAsync();
                ViewData["Message"] = "User.Identity is null - Not Authenticated so please login";
            }
            else
            {
                Log.Information(" just before AuthenticateAsync");
                // maybe chrome on a browser has just been opened
                // force authenticate try
                await HttpContext.AuthenticateAsync();

                ViewData["Message"] = "User.Identity.IsAuthentication is false - Not Authenticated so please login";
            }

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await AuthenticateUser(conn, Input.Email, Input.Password);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    //new Claim("FullName", user.FullName),
                    new Claim(ClaimTypes.Role,  user.CDRole)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                Log.Information($@"CDRole: {user.CDRole}");
                Log.Information($@"Remember me: {Input.RememberMe}");

                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    IsPersistent = Input.RememberMe, // false is default
                    //IsPersistent = true,
                    // Whether the authentication session is persisted across 
                    // multiple requests. When used with cookies, controls
                    // whether the cookie's lifetime is absolute (matching the
                    // lifetime of the authentication ticket) or session-based.

                    //IssuedUtc = <DateTimeOffset>,
                    // The time at which the authentication ticket was issued.

                    //RedirectUri = <string>
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                Log.Information($"User {user.Email} CDRole: {user.CDRole} logged in at {DateTime.UtcNow}");

                // creates a 302 Found which then redirects to the resource
                return LocalRedirect(returnUrl ?? "/");
            }

            // Something failed. Redisplay the form.
            return Page();
        }

        private async Task<Model?> AuthenticateUser(string connectionString, string email, string password)
        {
            await Task.Delay(500);

            // get login
            // TODO - put in sample user data and password into the insert scripts
            // also put in a register page

            //var login = await Db.LoginByEmail(connectionString, email);
            //if (login == null)
            //{
            //    Log.Information($"email address {email} not found in db");
            //    return null;
            //}

            //// check hash
            //var result = password.HashMatches(login.PasswordHash);
            //if (result == false)
            //{
            //    Log.Information($"password hashes don't match for email address {email}");
            //    return null;
            //}

            //Log.Information($"Successful password hash match for email {email} ");


            if (email == "tier1@contoso.com")
            {
                return new Model
                {
                    Email = "tier1@contoso.com",
                    CDRole = CDRole.Tier1
                };
            }

            if (email == "tier2@contoso.com")
            {
                return new Model
                {
                    Email = "tier2@contoso.com",
                    CDRole = CDRole.Tier2
                };
            }

            if (email == "admin@contoso.com")
            {
                return new Model
                {
                    Email = "admin@contoso.com",
                    CDRole = CDRole.Admin
                };
            }

            return null;
        }
    }
}
