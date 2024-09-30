using MTCG.BusinessLayer.Controller;
using MTCG.BusinessLayer.Model;
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

        public int Coins { get; set; } = 20;

        protected Deck Deck { get; set; } = new Deck();

        protected Stack Stack { get; set; } = new Stack();

        public void BuyPackage()
        {

            if (Coins >= Package.Price)
            {
                Stack.AddCards(CardController.Instance.GetCards(Package.MaxCards));
                Coins -= Package.Price;
            } else
            {
                Console.WriteLine("Not enough coins available");
            }
        }

        public void PrintStack()
        {
            Console.WriteLine("User's stack:");
            this.Stack.PrintCards();
        }
    }
}
