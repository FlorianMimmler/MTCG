
namespace MTCG
{
    internal class SpellCard : Card
    {
        public SpellCard(int damage, ElementType elementType, int? id = null, string? name = null) : base(name ?? elementType.GetString() + "Spell", damage, elementType, id)
        {
        }

    }
}

