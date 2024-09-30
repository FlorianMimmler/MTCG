﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.BusinessLayer
{
    internal interface ICalculateStrategy
    {
        double CalculateAgainst(Card opponent);
    }
}
