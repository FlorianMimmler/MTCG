using MTCG.BusinessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTCG
{
    internal abstract class CardWrapper : ICardWrapper
    {
        public List<ICard> Cards { get; set; }
        public virtual int MaxCards { get; set; }

        protected CardWrapper(int maxCards = 25)
        {
            this.MaxCards = maxCards;
            this.Cards = new List<ICard>(MaxCards);
        }

        public bool AddCard(ICard newCard)
        {
            if (this.Cards.Count >= MaxCards)
            {
                return false;
            }

            this.Cards.Add(newCard);

            return this.Cards.Contains(newCard);
        }

        public bool AddCards(List<ICard> newCards)
        {
            if (this.Cards.Count + newCards.Count > MaxCards)
            {
                return false;
            }

            this.Cards.AddRange(newCards);

            return newCards.All(card => this.Cards.Contains(card));
        }

        public bool RemoveCard(ICard oldCard)
        {
            return this.Cards.Remove(oldCard);
        }

        public ICard GetCard(string cardID)
        {
            return this.Cards.Find(card => card.Id == cardID);
        }

        public ICard GetRandomCard()
        {
            var rand = new Random();
            var index = rand.Next(this.Cards.Count);
            return this.Cards[index];
        }

        public void PrintCards()
        {
            foreach (var card in this.Cards)
            {
                Console.WriteLine(card.ToString());
            }
        }

        public bool IsEmpty()
        {
            return this.Cards.Count <= 0;
        }
    }
}
