using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    internal abstract class CardWrapper : ICardWrapper
    {
        public List<Card> Cards { get; set; }
        public virtual int MaxCards { get; set; }

        protected CardWrapper(int maxCards = 25)
        {
            this.MaxCards = maxCards;
            this.Cards = new List<Card>(MaxCards);
        }

        public bool AddCard(Card newCard)
        {
            if (this.Cards.Count >= MaxCards)
            {
                return false; 
            }

            this.Cards.Add(newCard);

            return this.Cards.Contains(newCard);
        }

        public bool AddCards(List<Card> newCards)
        {
            if (this.Cards.Count + newCards.Count > MaxCards)
            {
                return false;
            }

            this.Cards.AddRange(newCards);

            return newCards.All(card => this.Cards.Contains(card));
        }

        public bool RemoveCard(Card oldCard)
        {
            return this.Cards.Remove(oldCard);
        }

        public Card GetCard(int index)
        {
            return this.Cards[index];
        }

        public void PrintCards()
        {
            foreach (var card in this.Cards)
            {
                Console.WriteLine(card.ToString());   
            }
        }
    }
}
