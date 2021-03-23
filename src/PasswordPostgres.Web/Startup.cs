using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
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
        public void ConfigureServices(IServiceCollection services)
        {
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

            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
