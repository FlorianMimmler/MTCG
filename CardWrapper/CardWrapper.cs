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
        public int MaxCards { get; set; }

        protected CardWrapper(int maxCards = -1)
        {
            this.MaxCards = maxCards;
            this.Cards = new List<Card>(this.MaxCards);
        }

        public bool AddCard(Card newCard)
        {
            if (this.Cards.Count >= this.MaxCards)
            {
                return false; 
            }

            this.Cards.Add(newCard);

            return this.Cards.Contains(newCard);
        }

        public bool RemoveCard(Card oldCard)
        {
            return this.Cards.Remove(oldCard);
        }

        public Card GetCard(int index)
        {
            return this.Cards[index];
        }
    }
}
