using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace PasswordPostgres.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            Configuration = configuration;
        }

        private readonly IHostEnvironment _hostEnvironment;
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // persist cookies to disk so that if server restarts it can read them
            // and people wont have to login again if they have pressed remember me
            //https://stackoverflow.com/questions/56490525/asp-net-core-cookie-authentication-is-not-persistant
            // create a directory for keys if it doesn't exist
            // it'll be created in the root, beside the wwwroot directory
            var keysDirectoryName = "Keys";
            var keysDirectoryPath = Path.Combine(_hostEnvironment.ContentRootPath, keysDirectoryName);
            if (!Directory.Exists(keysDirectoryPath)) Directory.CreateDirectory(keysDirectoryPath);

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keysDirectoryPath))
                .SetApplicationName("CustomCookieAuthentication");

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            services.AddAuthorization();

            services.AddRazorPages();

            services.AddHttpContextAccessor();

            services.AddSingleton<IEmailService, EmailService>();

            // https://stackoverflow.com/questions/32201437/dapper-ambiguous-extension-methods
            //services.AddMiniProfiler(options => options.RouteBasePath = "/profiler");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging(); 

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseExceptionHandler("/CustomError");
            }

            //app.UseMiniProfiler();

            app.UseStaticFiles(); 

            // https://khalidabuhakmeh.com/handle-http-status-codes-with-razor-pages
            // https://andrewlock.net/retrieving-the-path-that-generated-an-error-with-the-statuscodepages-middleware/
            app.UseStatusCodePagesWithReExecute("/CustomError", "?statusCode={0}");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict });
            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
