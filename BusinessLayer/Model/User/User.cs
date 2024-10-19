﻿using MTCG.Auth;
using MTCG.BusinessLayer.Controller;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model;
using MTCG.BusinessLayer.Model.User;
using MTCG.DataAccessLayer;

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

        public User() {}

        public User(Credentials creds)
        {
            Credentials = creds;
        }

        public async Task<bool> BuyPackage()
        {

            if (Coins >= Package.Price)
            {
                var newCards = CardController.Instance.GetCards(Package.MaxCards);
                Stack.AddCards(newCards);
                Console.WriteLine("save cards");
                var result = await StackRepository.Instance.AddMultiple(newCards, Id);
                Console.WriteLine(result);
                if (!result)
                {
                    return false;
                }
                Coins -= Package.Price;
                return true;
            }
            else
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

        public async Task<bool> SelectDeck(string[] selection)
        {   
            Console.WriteLine(selection);
            _ = await StackRepository.Instance.ClearDeckFromUser(Id);

            var result = await StackRepository.Instance.SetDeckByCards(selection);
            return result == 4;

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
