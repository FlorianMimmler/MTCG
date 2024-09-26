using System;
using System.Collections.Generic;


namespace MTCG
{
    internal interface ICardWrapper
    {
        List<Card> Cards { get; set; }

        int MaxCards { get; set; }

        bool AddCard(Card newCard);

        bool RemoveCard(Card oldCard);

        Card GetCard(int index);

    }
}
