using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Auth
{
    internal class AuthenticationController
    {

        private static AuthenticationController _instance;
        public static AuthenticationController Instance => _instance ?? (_instance = new AuthenticationController());

        private AuthenticationController(){}


        public static AuthToken Login(Credentials creds)
        {
            if (creds == null)
            {
                return new AuthToken();
            }

            if (creds.Username == "username" && creds.Password == "password")
            {
                return new AuthToken();
            }

            return new AuthToken();
        }

        public static bool Signup(Credentials creds)
        {
            if (creds == null)
            {
                return false;
            }

            //perform signup

            return true;
        }

    }
}
