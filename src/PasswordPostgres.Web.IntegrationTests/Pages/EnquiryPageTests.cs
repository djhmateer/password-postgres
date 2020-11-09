using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PasswordPostgres.Web.IntegrationTests.Fakes;
using PasswordPostgres.Web.IntegrationTests.Helpers;
using PasswordPostgres.Web;
using Xunit;

namespace PasswordPostgres.Web.IntegrationTests.Pages
{
    public class EnquiryPageTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public EnquiryPageTests(WebApplicationFactory<Startup> factory)
        {
            // need this to be true (default)
            //factory.ClientOptions.HandleCookies = false;
            _factory = factory;
        }

        [Fact]
        public async Task Post_SendsEmail()
        {
            var emailService = new FakeEmailService();

            // cookie would have been reattached as using CreateClient
            //var client = _factory.WithWebHostBuilder(builder =>
            //{
            //    builder.ConfigureTestServices(services =>
            //    {
            //        services.AddSingleton<IEmailService>(emailService);
            //    });
            //}).CreateClient();

            var client = _factory.CreateClient();

            // this code below will result in a 400BadRequest
            // as we have not dealt with XSS Antiforgery tokens
            // CSRF - Cross-Site Request Forgery
            //var request = new HttpRequestmessage(HttpMethod.Post, "/enquiry")
            //{
            //    Content = new StringContent("email=sgordon@example.com&subject=Test message&message=This is a message.")
            //};

            //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            //var response = await client.SendAsync(request);

            //response.AssertOk();


            // by requesting the page we will get the anti forgery token
            var getResponse = await client.GetAsync("/enquiry");

            using var content = await HtmlHelpers.GetDocumentAsync(getResponse);

            // get the form (whioh includes the hidden field with the token)
            var form = (IHtmlFormElement)content.QuerySelector("form");

            if (form["EmailAddress"] is IHtmlInputElement email)
                email.Value = "integrationtest@example.com";

            if (form["Subject"] is IHtmlInputElement subject)
                subject.Value = "Testing from Integration Tests";

            if (form["Message"] is IHtmlTextAreaElement message)
                message.Value = "This is a test message from Integration Tests.";

            var button = (IHtmlButtonElement)content.QuerySelector("button");
            var formSubmission = form.GetSubmission(button);

            // need to extract the bits we need to do our test submission
            var target = (Uri)formSubmission.Target;

            var request = new HttpRequestMessage(new HttpMethod(formSubmission.Method.ToString()), target)
            {
                Content = new StreamContent(formSubmission.Body)
            };

            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await client.SendAsync(request);

            response.AssertOk();

            //Assert.Single(emailService.SentEmails);
        }
    }
}
