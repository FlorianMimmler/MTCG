using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    internal class MonsterCard : Card
    {
        public MonsterCard(string name, int damage, ElementType elementType, MonsterType monsterType) : base(name, damage, elementType)
        {
            this.MonsterType = monsterType;
        }

        public MonsterType MonsterType { get; set; }
        public override double CalculateDamageAgainst(Card opponent)
        {
            throw new NotImplementedException();
        }
    }
}
