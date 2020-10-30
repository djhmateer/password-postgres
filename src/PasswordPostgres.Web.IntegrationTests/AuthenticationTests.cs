using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace PasswordPostgres.Web.IntegrationTests
{
    public class AuthenticationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public AuthenticationTests(WebApplicationFactory<Startup> factory)
        {
            factory.ClientOptions.AllowAutoRedirect = false;
            _factory = factory;
        }

        [Theory]
        [InlineData("/Tier1RoleNeeded")]
        [InlineData("/Tier2RoleNeeded")]
        [InlineData("/AdminRoleNeeded")]
        [InlineData("/Crawl")]
        [InlineData("/Account/Manage/")]
        public async Task Get_SecurePageRedirectsAnUnauthenticatedUser(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("http://localhost/account/login", response.Headers.Location.OriginalString, StringComparison.OrdinalIgnoreCase);
        }
    }

}
