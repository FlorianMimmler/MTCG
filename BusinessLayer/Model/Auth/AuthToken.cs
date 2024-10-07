using System;

namespace MTCG.Auth
{
    internal class AuthToken
    {
        public string Value { get; set; } = "";
        public bool Valid { get; set; } = false;

        public AuthToken(bool valid = false) 
        {
            if (valid)
            {
                Value = Guid.NewGuid().ToString();
            }

            Valid = valid;
        }
    }
}
