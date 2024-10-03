﻿using MTCG.BusinessLayer.Controller;
using MTCG.BusinessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.User;

namespace MTCG
{
    internal class User
    {
        protected Credentials Credentials { get; set; }
        protected string Token { get; set; }

        public int Coins { get; set; } = 20;

        public Elo Elo = new Elo();

        protected ICardWrapper Deck { get; set; } = new Deck();

        protected ICardWrapper Stack { get; set; } = new Stack();

        public User(string username)
        {
            Credentials = new Credentials(username, "test");
        }

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

        public void PrintDeck()
        {
            Console.WriteLine("User's deck:");
            this.Deck.PrintCards();
        }

        public void SelectDeck(string selection)
        {
            foreach(var selectedCardIndex in selection.Split(';'))
            {
                AddCardToDeckFromStack(int.Parse(selectedCardIndex));
            }
        }

        private void AddCardToDeckFromStack(int stackIndex)
        {
            this.Deck.AddCard(this.Stack.GetCard(stackIndex));
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
    }
}
