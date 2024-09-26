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

        public int Coins { get; set; }

        protected ICardWrapper Deck { get; set; } = new Deck();

        protected ICardWrapper Stack { get; set; } = new Stack();
    }
}
