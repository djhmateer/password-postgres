using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace PasswordPostgres.Web
{
    public class AppConfiguration
    {
        public string ConnectionString { get; }
        public string PostmarkServerToken { get; }

        private AppConfiguration(
            string connectionString,
            string postmarkServerToken)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            PostmarkServerToken = postmarkServerToken ?? throw new ArgumentNullException(nameof(postmarkServerToken));
        }

        public static AppConfiguration LoadFromEnvironment()
        {
            // This reads the ASPNETCORE_ENVIRONMENT flag from the system

            // set on production server via the dot net run command
            // set on development via the launchSettings.json file
            // set on Unit test projects via the TestBase
            var aspnetcore = "ASPNETCORE_ENVIRONMENT";
            var env = Environment.GetEnvironmentVariable(aspnetcore);

            string connectionString;
            switch (env)
            {
                case "Development":
                case "Test":
                    connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json")
                        .Build().GetConnectionString("Default");
                    break;
                case "Production":
                    connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.Production.json")
                        .Build().GetConnectionString("Default");
                    break;
                default:
                    throw new ArgumentException($"Expected {nameof(aspnetcore)} to be Development, Test or Production and it is {env}");
            }


            // need to be more solid with try catch
            var filepath = Directory.GetCurrentDirectory();

            string postmarkServerToken;
            // https://postmarkapp.com/support/article/1213-best-practices-for-testing-your-emails-through-postmark
            if (env == "Test") postmarkServerToken = "POSTMARK_API_TEST";
            else if (filepath == "/var/www/web")
            {
                Log.Information("Linux looking for apikey for postmark");
                postmarkServerToken = File.ReadAllText(filepath + "/secrets/postmark-passwordpostgres.txt");
            }
            else
            {
                Log.Information("Windows looking for apikey for postmark");
                postmarkServerToken = File.ReadAllText("../../secrets/postmark-passwordpostgres.txt");
            }

            return new AppConfiguration(connectionString, postmarkServerToken);
        }
    }
}
