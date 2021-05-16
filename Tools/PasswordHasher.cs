using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.Tools
{
    public class PasswordHasher
    {

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="password">传值进来的密码</param>
        /// <param name="storePassword">对密码加密</param>
        /// <returns></returns>
        public static bool VerifyHashedPassword(string password, string storePassword)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (string.IsNullOrEmpty(storePassword))
            {
                throw new ArgumentNullException(nameof(storePassword));
            }

            var parts = storePassword.Split('.');
            var salt = parts[0];
            var hash = parts[1];

            return Validate(password, salt, hash);
        }

        /// <summary>
        /// 生成盐
        /// </summary>
        /// <returns></returns>
        private static string GenerateSalt()
        {
            byte[] randomBytes = new byte[128 / 8];
            var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private static bool Validate(string password, string salt, string hash)
             => HashPassword(password, salt) == hash;

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            var salt = GenerateSalt();
            var hash = HashPassword(password, salt);
            var result = $"{salt}.{hash}";
            // Console.WriteLine("hash result:{0}", result);
            return result;
        }
        private static string HashPassword(string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                password: value,
                salt: Encoding.UTF8.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return Convert.ToBase64String(valueBytes);
        }
    }
}
