using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using PasswordPostgres.Web.IntegrationTests.Helpers;
using Xunit;

namespace PasswordPostgres.Web.IntegrationTests
{
    public class ErrorTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ErrorTests(WebApplicationFactory<Startup> factory)
        {
            factory.ClientOptions.AllowAutoRedirect = false;
            _factory = factory;
        }

        [Theory]
        [InlineData("/asdfasdf")]
        [InlineData("/pagenothere")]
        public async Task Get_PageNotThere_ShouldReturn404AndCustomErrorPage(string url)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync(url);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            using var content = await HtmlHelpers.GetDocumentAsync(response);

            var h1 = content.QuerySelector("h1");

            // my custom 404 page - see startup for the handler
            Assert.Equal("Sorry - 404 Not Found", h1.TextContent);
        }

        [Fact]
        public async Task Get_ThrowException_ShouldReturn500()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/ThrowException");

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Get_ErrorPage_ShouldReturn200_ThisIsJustAPage_ThatOnProd_ErrorsAreRedirectedTo()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/Error");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

}
