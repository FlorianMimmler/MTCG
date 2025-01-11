﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer.Interface
{
    public interface ICardController
    {

        List<ICard> GetCards(int count);
        ICard GetCard();

    }
}
