using MTCG.BusinessLayer.Interface;
using System;
using System.Runtime.InteropServices;

namespace MTCG
{
    internal abstract class Card : ICard
    {
        protected Card(string name, int damage, ElementType elementType, int? id = null)
        {
            Id = id ?? -1;
            Name = name;
            Damage = damage;
            ElementType = elementType;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Damage { get; set; }
        public ElementType ElementType { get; set; }

        public override string ToString()
        {
            return $"Card: {Name}, Damage: {Damage}";
        }

    }
}
