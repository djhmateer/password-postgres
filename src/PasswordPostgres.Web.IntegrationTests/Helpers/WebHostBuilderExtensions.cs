using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace PasswordPostgres.Web.IntegrationTests.Helpers
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder WithAuthorisedUserInRole(this IWebHostBuilder builder, string role)
        {
            return builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication("Test")
                    .AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                        "Test", options => options.Role = role);
            });
        }
    }
}
