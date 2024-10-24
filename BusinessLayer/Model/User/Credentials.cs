
using System;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace MTCG
{
    internal class Credentials
    {
        public string Username { get; set; }
        public string Password { get; private set; }  // Password should be set only internally.
        public string Salt { get; private set; }

        public Credentials(string username, string password)
        {
            Username = username;
            Salt = GenerateSalt();  // Generate a random salt
            Password = HashPassword(password);
        }

        public Credentials() { }

        private string HashPassword(string password)
        {
            // Combine password and salt before hashing
            var saltedPassword = password + Salt;

            // Hash the salted password using SHA256
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hashBytes);
        }

        private string GenerateSalt()
        {
            var saltBytes = new byte[64];
            saltBytes = RandomNumberGenerator.GetBytes(saltBytes.Length);
            return Convert.ToBase64String(saltBytes);
        }

        public void HashPasswordWithSalt(string salt)
        {
            Salt = salt;
            Password = HashPassword(Password);
        }

        public void SetPassword(string password)
        {
            Password = password;
        }

        public void SetPasswordAndHash(string password)
        {
            Password = HashPassword(password);
        }

        public void SetSalt(string salt)
        {
            Salt = salt;
        }
    }
}
