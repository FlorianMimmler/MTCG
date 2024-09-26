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
        public Package() : base(5)
        {
            this.Cards = CardController.Instance.GetCards(MaxCards);
        }
    }
}
