using MTCG.BusinessLayer.Interface;
using System.Collections.Generic;


namespace MTCG
{
    public interface ICardWrapper
    {
        List<ICard> Cards { get; set; }
        bool AddCard(ICard newCard);
        bool AddCards(List<ICard> newCards);
        bool SetCards(List<ICard> newCards);
        bool RemoveCard(ICard oldCard);
        ICard? GetCard(int index);
        ICard GetRandomCard();
        bool IsEmpty();

    }
}
