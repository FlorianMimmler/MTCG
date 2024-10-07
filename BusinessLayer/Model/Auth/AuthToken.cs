﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Auth
{
    internal class AuthToken
    {
        public string Token { get; set; } = "";
        public bool Valid { get; set; } = false;

        public AuthToken(bool valid = false) 
        {
            if (valid)
            {
                Token = "generate token";
            }

            Valid = valid;
        }
    }
}
