using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    internal class SpellCard : Card
    {
        public SpellCard(int damage, ElementType elementType) : base(damage, elementType)
        {
        }

        public override double CalculateDamageAgainst(Card opponent)
        {
            throw new NotImplementedException();
        }
    }
}

