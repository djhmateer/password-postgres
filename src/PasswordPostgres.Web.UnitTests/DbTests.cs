using System;
using System.Threading.Tasks;
using Xunit;

namespace PasswordPostgres.Web.UnitTests
{
    public class DbTests : TestsBase
    {
        private readonly string connectionString;
        public DbTests() => connectionString = AppConfiguration.LoadConnectionStringFromEnvironment().ConnectionString;

        [Fact]
        public async Task ShouldBeAbleToInsertLogin()
        {
            var login = new Login
            {
                Email = $"{Guid.NewGuid().ToString()}@mailinator.com",
                PasswordHash = Guid.NewGuid().ToString()
            };

            var returnedLogin = await Db.InsertLogin(connectionString, login);

            Assert.Equal(login.Email, returnedLogin.Email);
            Assert.Equal(login.PasswordHash, returnedLogin.PasswordHash);
            Assert.True(returnedLogin.LoginId > 0);
        }

        [Fact]
        public async Task ShouldBeAbleToGetLoginByEmail()
        {
            var login = new Login
            {
                Email = $"{Guid.NewGuid().ToString()}@mailinator.com",
                PasswordHash = Guid.NewGuid().ToString(),
                Verified = true
            };

            await Db.InsertLogin(connectionString, login);
            Login? result = await Db.LoginByEmail(connectionString, login.Email);

            Assert.NotNull(result);
            var returnedLogin = result;
            Assert.Equal(login.Email, returnedLogin.Email);
            Assert.Equal(login.PasswordHash, returnedLogin.PasswordHash);
            Assert.True(returnedLogin.LoginId > 0);
        }

        [Fact]
        public async Task ShouldBeAbleToUpdatePassword()
        {
            var login = new Login
            {
                Email = $"{Guid.NewGuid().ToString()}@mailinator.com",
                PasswordHash = Guid.NewGuid().ToString(),
                Verified = true
            };

            await Db.InsertLogin(connectionString, login);
            var result = await Db.LoginByEmail(connectionString, login.Email);
            Assert.NotNull(result);

            var newPasswordHash = Guid.NewGuid().ToString();
            await Db.UpdateLoginPassword(connectionString, result.LoginId, newPasswordHash);

            var loginByIdResult = await Db.LoginById(connectionString, result.LoginId);

            Assert.Equal(newPasswordHash, loginByIdResult.PasswordHash);
        }

    }
}
