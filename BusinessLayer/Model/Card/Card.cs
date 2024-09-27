using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    internal abstract class Card
    {
        protected Card(string name, int damage, ElementType elementType)
        {
            Name = name;
            Damage = damage;
            ElementType = elementType;
        }

        protected string Name { get; set; }
        protected int Damage { get; set; }
        protected ElementType ElementType { get; set; }

        public abstract double CalculateDamageAgainst(Card opponent);

        public override string ToString()
        {
            return $"Card: {Name}, Damage: {Damage}";
        }

    }
}
