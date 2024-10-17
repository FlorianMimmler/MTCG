
using System;
using System.Security.Cryptography;
using System.Security.Policy;

namespace MTCG
{
    internal class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public byte[] Salt { get; set; }

        public Credentials(string username, string password)
        {
            Username = username;
            HashPassword(password);
        }

        private void HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            Salt = salt;

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
            var hash = pbkdf2.GetBytes(20);

            var hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            Password = Convert.ToBase64String(hashBytes);


        }
    }
}
