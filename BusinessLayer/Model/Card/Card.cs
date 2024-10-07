using MTCG.BusinessLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    internal abstract class Card : ICard
    {
        protected Card(string name, int damage, ElementType elementType)
        {
            Name = name;
            Damage = damage;
            ElementType = elementType;
        }

        public string Name { get; set; }
        public int Damage { get; set; }
        public ElementType ElementType { get; set; }

        public override string ToString()
        {
            return $"Card: {Name}, Damage: {Damage}";
        }

    }
}
