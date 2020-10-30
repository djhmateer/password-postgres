using Microsoft.Extensions.Configuration;
using Xunit;

namespace PasswordPostgres.Web.UnitTests
{
    public class EmailTests
    {
        [Fact]
        public void ShouldBeAbleToSendAnEmail()
        {
            //var AppName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
            //    .GetSection("AppSettings")["APP_Name"];

            var emailConfiguration = AppConfiguration.LoadFromEnvironment().GetEmailConfiguration();

            var message = new EmailMessage
            {
                ToAddress = "tardis-bank@mailinator.com",
                Subject = "Hello from the Email integration test",
                Body = "Hi Mike, Congratulations, the email thingy works fine. Mike"
            };

            //Email.Send(emailConfiguration, message);
        }

    }
}
