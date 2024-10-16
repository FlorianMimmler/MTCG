using System;
using System.Runtime.InteropServices;
using MTCG.BusinessLayer.Interface;

namespace MTCG
{
    internal abstract class Card : ICard
    {
        protected Card(string name, int damage, ElementType elementType)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Damage = damage;
            ElementType = elementType;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int Damage { get; set; }
        public ElementType ElementType { get; set; }

        public override string ToString()
        {
            return $"Card: {Name}, Damage: {Damage}";
        }

    }
}
