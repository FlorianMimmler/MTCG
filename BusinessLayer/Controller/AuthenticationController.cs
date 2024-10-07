using System.Collections.Generic;
using System.Linq;

namespace MTCG.Auth
{
    internal class AuthenticationController
    {

        private static AuthenticationController _instance;
        public static AuthenticationController Instance => _instance ??= new AuthenticationController();

        private AuthenticationController()
        {
            this._users = new List<User>();
        }

        /* ONLY FOR USAGE WITHOUT DB */

        private List<User> _users;

        /* _____ */


        public AuthToken Login(Credentials creds)
        {
            if (creds == null)
            {
                return new AuthToken();
            }

            var user = this._users.FirstOrDefault(u => u.GetName() == creds.Username);

            if (user == null)
            {
                return new AuthToken();
            }

            if (!user.IsPasswordEqual(creds.Password)) return new AuthToken();
            {
                var authToken = new AuthToken(true);
                this._users.Find(u => u.GetName() == creds.Username).Token = authToken;
                return authToken;
            }

        }

        public bool Signup(Credentials creds)
        {
            var user = this._users.FirstOrDefault(u => u.GetName() == creds.Username);

            if (user != null)
            {
                return false;
            }

            var newUser = new User(creds);
            this._users.Add(newUser);

            return true;
        }

        public bool IsAuthorized(string authToken)
        {
            var user = this._users.FirstOrDefault(u => u.Token.Value == authToken);

            return user != null;

        }

        public User GetUserByToken(string authToken)
        {
            var user = this._users.FirstOrDefault(u => u.Token.Value == authToken);

            return user;
        }

    }
}
