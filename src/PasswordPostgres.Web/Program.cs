using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace PasswordPostgres.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                // gets rid of lots of noise
                // filter out /healthcheck
                //.Filter.ByExcluding("RequestPath like '/health%'")
                //.Filter.ByExcluding("SourceContext <> 'Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware'")
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                //.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Verbose)
                // if only do warning, then will get duplicate error messages when an exception is thrown, then again when re-executed
                // we do get 2 error message per single error, but only 1 stack trace
                //.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Fatal)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                // use c:/logs/passwordpostgres/ sometimes if can't control permissions on prod 
                .WriteTo.File(@"logs/warning.txt", restrictedToMinimumLevel: LogEventLevel.Warning, rollingInterval: RollingInterval.Day)
                .WriteTo.File(@"logs/info.txt", restrictedToMinimumLevel: LogEventLevel.Information, rollingInterval: RollingInterval.Day)
                //.WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341")

                //.Filter.ByExcluding("SourceContext = 'Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware'")
                //.WriteTo.File(@"logs/warning2.txt", restrictedToMinimumLevel: LogEventLevel.Warning, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog() // <- Add this line
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
