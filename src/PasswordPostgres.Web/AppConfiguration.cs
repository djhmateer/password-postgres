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

        private AppConfiguration(string connectionString) =>
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

        public static AppConfiguration LoadConnectionStringFromEnvironment()
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

        public string GetPostmarkServerKey()
        {
            // need to be more solid with try catch
            var filepath = Directory.GetCurrentDirectory();

            string apiKey;
            if (filepath == "/var/www/web")
            {
                Log.Information("Linux looking for apikey for postmark");
                apiKey = File.ReadAllText(filepath + "/secrets/postmark-passwordpostgres.txt");
            }
            else
            {
                Log.Information("Windows looking for apikey for postmark");
                apiKey = File.ReadAllText("../../secrets/postmark-passwordpostgres.txt");
            }

            return apiKey;
        }
    }

    //public class EmailConfiguration
    //{
    //    public string SmtpServer { get; }
    //    public int SmtpServerPort { get; }
    //    public bool UseSSL { get; }
    //    public string SmtpUsername { get; }
    //    public string SmtpPassword { get; }

    //    public EmailConfiguration(
    //        string smtpServer,
    //        int smtpServerPort,
    //        bool useSSL,
    //        string smtpUsername,
    //        string smtpPassword)
    //    {
    //        SmtpServer = smtpServer ?? throw new ArgumentNullException(nameof(smtpServer));
    //        SmtpServerPort = smtpServerPort;
    //        UseSSL = useSSL;
    //        SmtpUsername = smtpUsername ?? throw new ArgumentNullException(nameof(smtpUsername));
    //        SmtpPassword = smtpPassword ?? throw new ArgumentNullException(nameof(smtpPassword));
    //    }
    //}
}
