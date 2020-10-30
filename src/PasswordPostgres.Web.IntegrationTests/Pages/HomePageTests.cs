using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using PasswordPostgres.Web.IntegrationTests.Helpers;
using Xunit;

namespace PasswordPostgres.Web.IntegrationTests.Pages
{
    public class HomePageTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public HomePageTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ReturnsPageWithExpectedH1()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/");

            response.AssertOk();

            using var content = await HtmlHelpers.GetDocumentAsync(response);

            var h1 = content.QuerySelector("h1");

            Assert.Equal("Welcome to PasswordPostgres", h1.TextContent);
        }
    }
}
