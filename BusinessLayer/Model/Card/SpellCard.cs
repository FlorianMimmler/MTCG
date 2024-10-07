
namespace MTCG
{
    internal class SpellCard : Card
    {
        public SpellCard(int damage, ElementType elementType) : base(elementType.GetString() + "Spell", damage, elementType)
        {
        }

    }
}

