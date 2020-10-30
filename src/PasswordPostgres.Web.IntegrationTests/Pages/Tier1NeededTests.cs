using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using PasswordPostgres.Web.IntegrationTests.Helpers;
using Xunit;

namespace PasswordPostgres.Web.IntegrationTests.Pages
{
    public class Tier1NeededTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public Tier1NeededTests(WebApplicationFactory<Startup> factory)
        {
            factory.ClientOptions.AllowAutoRedirect = false;
            _factory = factory;
        }

        public static IEnumerable<object[]> RoleAccess => new List<object[]>
        {
            new object[] { CDRole.Tier1, HttpStatusCode.OK },
            new object[] { CDRole.Tier2, HttpStatusCode.OK },
            new object[] { CDRole.Admin, HttpStatusCode.OK },
            new object[] { "anewrole", HttpStatusCode.Forbidden }
        };

        [Theory]
        [MemberData(nameof(RoleAccess))]
        public async Task Get_SecurePageAccessibleOnlyByAdminUsers(string role, HttpStatusCode expected)
        {
            var client = _factory
                .WithWebHostBuilder(x => x.WithAuthorisedUserInRole(role)) // this is my extension method
                .CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            var response = await client.GetAsync("/Tier1RoleNeeded");

            Assert.Equal(expected, response.StatusCode);
        }
    }
}
