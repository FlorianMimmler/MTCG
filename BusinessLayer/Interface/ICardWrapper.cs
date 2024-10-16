using System.Collections.Generic;
using MTCG.BusinessLayer.Interface;


namespace MTCG
{
    internal interface ICardWrapper
    {
        List<ICard> Cards { get; set; }

        bool AddCard(ICard newCard);

        bool AddCards(List<ICard> newCards);

        bool RemoveCard(ICard oldCard);

        ICard GetCard(string index);
        ICard GetRandomCard();

        void PrintCards();

        bool IsEmpty();

    }
}
