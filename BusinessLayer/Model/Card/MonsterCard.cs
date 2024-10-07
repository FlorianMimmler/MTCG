
namespace MTCG
{
    internal class MonsterCard : Card
    {
        public MonsterCard(int damage, ElementType elementType, MonsterType monsterType) : base(elementType.GetString() + monsterType, damage, elementType)
        {
            this.MonsterType = monsterType;
        }
        public MonsterType MonsterType { get; set; }

    }
}
