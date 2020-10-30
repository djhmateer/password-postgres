using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using PasswordPostgres.Web.Pages;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace PasswordPostgres.Web
{
    public static class Db
    {
        public static async Task<Login> InsertLogin(string connectionString, Login login)
        {
            using var conn = GetOpenConnection(connectionString);

            var result = await conn.QueryAsync<Login>(
                "INSERT INTO login (email, password_hash, verified) " +
                "VALUES (@Email, @PasswordHash, @Verified) " +
                "RETURNING login_id as LoginId, email as Email, password_hash as PasswordHash, " +
                "verified as Verified",
                login);

            return result.Single();
        }


        public static async Task<Login?> LoginByEmail(string connectionString, string email)
        {
            using var conn = GetOpenConnection(connectionString);

            var result = await conn.QueryAsync<Login>(
                "SELECT login_id as LoginId, email as Email, password_hash as PasswordHash " +
                "FROM login WHERE email = @Email AND verified",
                new { Email = email });

            if (result.Any())
            {
                return result.Single();
            }

            return null;
        }

        public static async Task UpdateLoginPassword(string connectionString, int loginId, string newPasswordHash)
        {
            using var conn = GetOpenConnection(connectionString);

            var result = await conn.QueryAsync<Login>(
                "UPDATE login SET password_hash = @NewPasswordHash WHERE login_id = @LoginId",
                new { LoginId = loginId, NewPasswordHash = newPasswordHash });
        }

        public static async Task<Login> LoginById(string connectionString, int loginId)
        {
            using var conn = GetOpenConnection(connectionString);

            var result = await conn.QueryAsync<Login>(
                "SELECT login_id as LoginId, email as Email, password_hash as PasswordHash " +
                "FROM login WHERE login_id = @LoginId",
                new { LoginId = loginId });

            if (result.Any())
            {
                return result.Single();
            }
            // throw rather than returning Maybe<Login>. Any id based lookup
            // should succeed, so this a server fault and should be thrown.
            throw new ApplicationException("LoginId not found in DB.");
        }

        // Cassini
        public static async Task<IEnumerable<Thing>> GetThings(string connectionString)
        {
            using var conn = GetOpenConnection(connectionString);

            var result = await conn.QueryAsync<Thing>(
                @"SELECT id as Id, date as Date, team as Team, target as Target,
                 title as Title, description as Description
                 FROM master_plan LIMIT 10");

            return result;
        }

        // DBTest
        public static async Task<IEnumerable<Employee>> GetEmployees(string connectionString)
        {
            using var conn = GetOpenConnection(connectionString);

            var result = await conn.QueryAsync<Employee>(
                "SELECT first_name as FirstName, last_name as LastName, address as Address " +
                "FROM employee");

            return result;
        }

        public static IDbConnection GetOpenConnection(string connectionString)
        {
            if (connectionString == null) throw new ArgumentException("connectionString can't be null");

            DbConnection cnn = new NpgsqlConnection(connectionString);
            if (MiniProfiler.Current != null)
            {
                cnn = new ProfiledDbConnection(cnn, MiniProfiler.Current);
            }
            cnn.Open();
            return cnn;
        }
    }
}
