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
        //public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        //{
        //    this.WebHostEnvironment = webHostEnvironment;
        //    Configuration = configuration;
        //}

        //public IConfiguration Configuration { get; }
        //public IWebHostEnvironment WebHostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            services.AddAuthorization();

            services.AddRazorPages();

            services.AddHttpContextAccessor();

            services.AddSingleton<IEmailService, EmailService>();

            // https://stackoverflow.com/questions/32201437/dapper-ambiguous-extension-methods
            services.AddMiniProfiler(options => options.RouteBasePath = "/profiler");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseMiniProfiler();

            // https://joonasw.net/view/custom-error-pages
            app.Use(async (ctx, next) =>
            {
                await next();

                if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
                {
                    //Re-execute the request so the user gets the error page
                    string originalPath = ctx.Request.Path.Value;
                    ctx.Items["originalPath"] = originalPath;
                    ctx.Request.Path = "/errors/404";
                    await next();
                }
            });

            app.UseStaticFiles();
            app.UseRouting();

            // don't want request logging for static files so put it here in the pipeline
            app.UseSerilogRequestLogging();

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
