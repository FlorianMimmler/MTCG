
using System;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace MTCG
{
    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; private set; }
        public string Salt { get; private set; }

        public Credentials(string username, string password)
        {
            Username = username;
            Salt = GenerateSalt();
            Password = HashPassword(password);
        }

        public Credentials() 
        {
            Username = "";
            Password = "";
            Salt = "";
        }

        private string HashPassword(string password)
        {
            var saltedPassword = password + Salt;

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
