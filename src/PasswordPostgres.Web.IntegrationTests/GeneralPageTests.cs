using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Net.Http.Headers;
using PasswordPostgres.Web.IntegrationTests.Helpers;
using Xunit;

namespace PasswordPostgres.Web.IntegrationTests
{
    public class GeneralPageTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public GeneralPageTests(WebApplicationFactory<Startup> factory)
        {
            factory.ClientOptions.AllowAutoRedirect = false;
            _factory = factory;
        }

        // should be only pages that don't need auth
        public static IEnumerable<object[]> ValidUrls => new List<object[]>
        {
            new object[] {"/"},
            new object[] {"/Index"},
            new object[] {"/healthcheck"},
            new object[] {"/Anonymous"},
            new object[] {"/Enquiry"}
        };

        [Theory]
        [MemberData(nameof(ValidUrls))]
        public async Task ValidUrls_ReturnSuccessAndExpectedContentType(string path)
        {
            var expected = new MediaTypeHeaderValue("text/html");

            var client = _factory.CreateClient();

            var response = await client.GetAsync(path);

            response.AssertOk();

            Assert.Equal(expected.MediaType, response.Content.Headers.ContentType.MediaType);
        }
    }
}
