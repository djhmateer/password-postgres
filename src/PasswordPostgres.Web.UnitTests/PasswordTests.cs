﻿using System;
using Xunit;

namespace PasswordPostgres.Web.UnitTests
{
    public class PasswordTests
    {
        [Fact]
        public void ShouldBeAbleToHashAndCheckPassword()
        {
            var password = Guid.NewGuid().ToString();

            var hash = password.HashPassword();
            var isMatch = password.HashMatches(hash);

            Assert.True(isMatch);
        }
    }
}
