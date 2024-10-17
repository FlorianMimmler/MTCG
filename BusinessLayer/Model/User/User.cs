using MTCG.BusinessLayer.Controller;
using MTCG.BusinessLayer.Model;
using System;
using System.Linq;
using MTCG.Auth;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.User;

namespace MTCG
{
    internal class User
    {
        public int Id { get; set; }
        public Credentials Credentials { get; set; }
        public AuthToken Token { get; set; }
        public bool Admin { get; set; } = true;

        public int Coins { get; set; } = 20;

        public Stats Stats = new Stats();

        public ICardWrapper Deck { get; set; } = new Deck();

        public ICardWrapper Stack { get; set; } = new Stack();

        public User(string username)
        {
            Credentials = new Credentials(username, "test");
        }

        public User(Credentials creds)
        {
            Credentials = creds;
        }

        public bool BuyPackage()
        {

            if (Coins >= Package.Price)
            {
                Stack.AddCards(CardController.Instance.GetCards(Package.MaxCards));
                Coins -= Package.Price;
                return true;
            } else
            {
                Console.WriteLine("Not enough coins available");
                return false;
            }
        }

        public void PrintStack()
        {
            Console.WriteLine("User's stack:");
            this.Stack.PrintCards();
        }

        public void PrintDeck()
        {
            Console.WriteLine("User's deck:");
            this.Deck.PrintCards();
        }

        public bool SelectDeck(string[] selection)
        {
            this.Deck.Cards.Clear();

            try
            {
                return selection.All(AddCardToDeckFromStack);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
                
            }
        }

        private bool AddCardToDeckFromStack(string cardID)
        {
            var cardToAdd = this.Stack.GetCard(cardID);

            if (cardToAdd == null)
            {
                return false;
            }

            this.Deck.AddCard(cardToAdd);

            return true;
        }

        public ICard GetRandomCardFromDeck()
        {
            return this.Deck.GetRandomCard();
        }

        public ICardWrapper GetDeck()
        {
            return this.Deck;
        }

        public string GetName()
        {
            return this.Credentials.Username;
        }

        public bool IsPasswordEqual(string password)
        {
            return this.Credentials.Password == password;
        }
    }
}
