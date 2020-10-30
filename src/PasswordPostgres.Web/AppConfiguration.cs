using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace PasswordPostgres.Web
{
    public class AppConfiguration
    {
        public string ConnectionString { get; }

        private AppConfiguration(string connectionString) =>
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

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
            return new AppConfiguration(connectionString);
        }

        public EmailConfiguration GetEmailConfiguration()
        {
            var sendgrid_api_key = System.IO.File.ReadAllText("sendgrid-passwordpostgres.txt");
            //var serverParts = SmtpServer.Split(':');
            //int serverPort = 0;
            //const string errorMessage = "Misconfigured TARDISBANK_SMTP_SERVER. Expected '<server>:<port>:<use HTTPS>[true|false]'.";
            //if (serverParts.Length != 3)
            //{
            //    throw new ApplicationException(errorMessage);
            //}
            //if (!int.TryParse(serverParts[1], out serverPort))
            //{
            //    throw new ApplicationException(errorMessage + " Could not parse <port> as integer.");
            //}
            //if (serverParts[2] != "true" && serverParts[2] != "false")
            //{
            //    throw new ApplicationException(errorMessage + " Could not parse <use HTTPS> as bool, expected 'true' or 'false'.");
            //}
            //var useSLL = serverParts[2] == "true";


            //var credentialParts = SmtpCredentials.Split(':');
            //if (credentialParts.Length != 2)
            //{
            //    throw new ApplicationException("Misconfigured TARDISBANK_SMTP_CREDENTIALS. Expected '<username>:<password>'.");
            //}

            //return new EmailConfiguration(
            //    serverParts[0],
            //    serverPort,
            //    useSLL,
            //    credentialParts[0],
            //    credentialParts[1]);
            return null;
        }

    }

    public class EmailConfiguration
    {
        public string SmtpServer { get; }
        public int SmtpServerPort { get; }
        public bool UseSSL { get; }
        public string SmtpUsername { get; }
        public string SmtpPassword { get; }

        public EmailConfiguration(
            string smtpServer,
            int smtpServerPort,
            bool useSSL,
            string smtpUsername,
            string smtpPassword)
        {
            SmtpServer = smtpServer ?? throw new ArgumentNullException(nameof(smtpServer));
            SmtpServerPort = smtpServerPort;
            UseSSL = useSSL;
            SmtpUsername = smtpUsername ?? throw new ArgumentNullException(nameof(smtpUsername));
            SmtpPassword = smtpPassword ?? throw new ArgumentNullException(nameof(smtpPassword));
        }
    }
}
