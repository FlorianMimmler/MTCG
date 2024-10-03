using System;
using System.Collections.Generic;


namespace MTCG
{
    internal interface ICardWrapper
    {
        List<Card> Cards { get; set; }

        bool AddCard(Card newCard);

        bool AddCards(List<Card> newCards);

        bool RemoveCard(Card oldCard);

        Card GetCard(int index);

        void PrintCards();

    }
}
