using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer.Controller;

namespace MTCG.BusinessLayer.Model
{
    internal class Package : CardWrapper
    {
        public static int Price { get; set; } = 5;
        public new static int MaxCards { get; set; } = 5;
        public Package() : base(MaxCards)
        {
            this.Cards = CardController.Instance.GetCards(MaxCards);
        }

        

    }
}
