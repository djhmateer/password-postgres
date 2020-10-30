using System.Threading.Tasks;
using PasswordPostgres.Web.IntegrationTests.Helpers;
using Xunit;

namespace PasswordPostgres.Web.IntegrationTests
{
    // Base CustomWebApplicationFactory class sets the ASPNETCORE_ENVIRONMENT to Test
    public class GeneralHittingDb : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public GeneralHittingDb(CustomWebApplicationFactory<Startup> factory)
        {
            factory.ClientOptions.AllowAutoRedirect = false;
            _factory = factory;
        }

        [Fact]
        public async Task Get_HostingEnvironment_ShouldBeTest()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/HostingEnvironment");

            response.AssertOk();
            using var content = await HtmlHelpers.GetDocumentAsync(response);

            var h1 = content.QuerySelector("h2");
            Assert.Equal("Test", h1.TextContent);
        }

        [Fact]
        public async Task Get_DBTest_Returns200()
        {
            var client = _factory.CreateClient();

            // Base class sets the env variable, which in the Web.AppConfiguration reads the ConnectionString 
            // from the web project - using appsettings.Development.json db for Test
            var response = await client.GetAsync("/DBTest");

            response.AssertOk();
        }
    }
}
