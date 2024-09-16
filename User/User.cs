using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    internal class User
    {
        protected Credentials Credentials { get; set; }
        protected string Token { get; set; }

        public int coins { get; set; }

        protected Deck Deck { get; set; }

        protected Stack Stack { get; set; }
    }
}
