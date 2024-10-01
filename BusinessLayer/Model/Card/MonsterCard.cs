using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.BusinessLayer;

namespace MTCG
{
    internal class MonsterCard : Card
    {
        public MonsterCard(int damage, ElementType elementType, MonsterType monsterType) : base(elementType.GetString() + monsterType.ToString(), damage, elementType)
        {
            this.MonsterType = monsterType;
        }

        private ICalculateStrategy CalculateStrategy;
        public MonsterType MonsterType { get; set; }
        public override double CalculateDamageAgainst(Card opponent)
        {

            throw new NotImplementedException();
        }
    }
}
