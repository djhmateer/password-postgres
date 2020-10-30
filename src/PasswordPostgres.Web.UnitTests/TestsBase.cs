using System;

namespace PasswordPostgres.Web.UnitTests
{
    public class TestsBase
    {
        public TestsBase() => 
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
    }
}