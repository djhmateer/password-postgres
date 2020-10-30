using System;
using System.Linq;
using System.Security.Cryptography;

namespace PasswordPostgres.Web
{
    public static class Password
    {
        public static string HashPassword(this string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            var saltSize = 16; // 128 bit
            var keySize = 32; // 256 bit
            var iterations = 10000;

            using var algorithm = new Rfc2898DeriveBytes(password, saltSize, iterations, HashAlgorithmName.SHA512);
            var key = Convert.ToBase64String(algorithm.GetBytes(keySize));
            var salt = Convert.ToBase64String(algorithm.Salt);

            return $"{iterations}.{salt}.{key}";
        }

        public static bool HashMatches(this string password, string hashedPassword)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (hashedPassword == null) throw new ArgumentNullException(nameof(hashedPassword));

            var keySize = 32; // 256 bit

            var parts = hashedPassword.Split('.', 3);

            if (parts.Length != 3)
            {
                throw new FormatException("Unexpected hash format. " +
                                          "Should be formatted as `{iterations}.{salt}.{hash}`");
            }

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            using var algorithm = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512);
            var keyToCheck = algorithm.GetBytes(keySize);

            var verified = keyToCheck.SequenceEqual(key);

            return verified;
        }






        //public static string HashPassword(this string password)
        //{
        //    if (password == null) throw new ArgumentNullException(nameof(password));
        //    var saltBytes = GenerateSalt(4);

        //    return HashPassword(password, saltBytes);
        //}

        //public static bool HashMatches(this string password, string hashedPassword)
        //{
        //    if (password == null) throw new ArgumentNullException(nameof(password));
        //    if (hashedPassword == null) throw new ArgumentNullException(nameof(hashedPassword));

        //    var saltBytes = GetSaltBytes(hashedPassword);
        //    var newHash = HashPassword(password, saltBytes);

        //    return newHash == hashedPassword;
        //}

        //private static string HashPassword(string password, byte[] saltBytes)
        //{
        //    var algorithm = new SHA1Managed();
        //    var plainTextBytes = Encoding.UTF8.GetBytes(password);

        //    var plainTextWithSaltBytes = AppendByteArray(plainTextBytes, saltBytes);
        //    var saltedSHA1Bytes = algorithm.ComputeHash(plainTextWithSaltBytes);
        //    var saltedSHA1WithAppendedSaltBytes = AppendByteArray(saltedSHA1Bytes, saltBytes);

        //    return Convert.ToBase64String(saltedSHA1WithAppendedSaltBytes);
        //}

        //private static byte[] GetSaltBytes(string hashedPassword)
        //{
        //    var hashedBytes = Convert.FromBase64String(hashedPassword);
        //    return hashedBytes.AsSpan().Slice(hashedBytes.Length - 4).ToArray();
        //}

        //private static byte[] GenerateSalt(int saltSize)
        //{
        //    var rng = new RNGCryptoServiceProvider();
        //    var buff = new byte[saltSize];
        //    rng.GetBytes(buff);
        //    return buff;
        //}

        //private static byte[] AppendByteArray(byte[] byteArray1, byte[] byteArray2)
        //{
        //    var byteArrayResult =
        //            new byte[byteArray1.Length + byteArray2.Length];

        //    for (var i = 0; i < byteArray1.Length; i++)
        //        byteArrayResult[i] = byteArray1[i];
        //    for (var i = 0; i < byteArray2.Length; i++)
        //        byteArrayResult[byteArray1.Length + i] = byteArray2[i];

        //    return byteArrayResult;
        //}

    }
}
