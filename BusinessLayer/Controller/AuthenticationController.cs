using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

            return user.IsPasswordEqual(creds.Password) ? new AuthToken(true) : new AuthToken();
        }

        public bool Signup(Credentials creds)
        {
            var user = this._users.FirstOrDefault(u => u.GetName() == creds.Username);

            if (user != null)
            {
                return false;
            }

            //perform signup
            var newUser = new User(creds);
            this._users.Add(newUser);

            return true;
        }

    }
}
