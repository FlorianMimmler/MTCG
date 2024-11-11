
namespace MTCG
{
    internal class MonsterCard : Card
    {
        public MonsterCard(int damage, ElementType elementType, MonsterType monsterType, int? id = null, string? name = null) : base(name ?? elementType.GetString() + monsterType, damage, elementType, id)
        {
            this.MonsterType = monsterType;
        }

        public MonsterType MonsterType { get; set; }

    }
}
