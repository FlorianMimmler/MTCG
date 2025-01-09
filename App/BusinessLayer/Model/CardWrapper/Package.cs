
using MTCG.BusinessLayer.Controller;

namespace MTCG.BusinessLayer.Model.CardWrapper
{
    public class Package : CardWrapper
    {
        public static int Price { get; set; } = 5;
        public new static int MaxCards { get; set; } = 5;
        public Package() : base(MaxCards)
        {
            this.Cards = CardController.Instance.GetCards(MaxCards);
        }



    }
}
