using MTCG.BusinessLayer.Interface;
using System;
using System.Runtime.InteropServices;

namespace MTCG
{
    internal abstract class Card : ICard
    {
        protected Card(string name, int damage, ElementType elementType, string? id = null)
        {
            Id = id ?? Guid.NewGuid().ToString();
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
