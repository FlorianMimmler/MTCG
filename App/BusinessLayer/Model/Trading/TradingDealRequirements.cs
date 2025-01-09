using MTCG.BusinessLayer.Model.Card;

namespace MTCG.BusinessLayer.Model.Trading
{
    internal class TradingDealRequirements
    {

        public int CardType { get; set; }
        public ElementType? ElementType { get; set; }
        public MonsterType? MonsterType { get; set; }
        public int MinDamage { get; set; }

    }
}
