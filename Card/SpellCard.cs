using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Card
{
    internal class SpellCard : Card
    {
        public SpellCard(string name, int damage, ElementType elementType) : base(name, damage, elementType)
        {
        }

        public override double CalculateDamageAgainst(Card opponent)
        {
            throw new NotImplementedException();
        }
    }
}

