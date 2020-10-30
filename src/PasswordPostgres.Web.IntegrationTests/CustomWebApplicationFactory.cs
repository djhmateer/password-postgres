using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PasswordPostgres.Web.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //builder.UseEnvironment("Development");

            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
            //base.ConfigureWebHost(builder);
        }
    }
}
