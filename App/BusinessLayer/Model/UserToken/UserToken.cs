using MTCG.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    public class UserToken(int userID, AuthToken token)
    {

        public int UserID = userID;

        public AuthToken Token = token;
    }
}
