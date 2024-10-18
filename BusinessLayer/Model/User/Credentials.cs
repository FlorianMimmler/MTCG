
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
            HashPassword(password);  // Hash the password with the salt
        }

        private void HashPassword(string password)
        {
            // Combine password and salt before hashing
            string saltedPassword = password + Salt;

            // Hash the salted password using SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                Password = Convert.ToBase64String(hashBytes);  // Convert the hash to Base64 for easier storage
            }
        }

        private string GenerateSalt()
        {
            // Generate a cryptographically secure random salt
            byte[] saltBytes = new byte[16]; // 128-bit salt
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            // Convert salt to a string for storage (e.g., Base64 encoding)
            return Convert.ToBase64String(saltBytes);
        }
    }
}
