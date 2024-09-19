using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Auth
{
    internal class Authentication
    {

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
