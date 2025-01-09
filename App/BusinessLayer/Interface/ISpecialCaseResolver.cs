using MTCG.BusinessLayer.Model.BattleStrategy;
using MTCG.BusinessLayer.Model.Card;

namespace MTCG.BusinessLayer.Interface
{
    public interface ISpecialCaseResolver
    {
        BattleResult Resolve(MonsterCard? card1, MonsterCard? card2);

    }
}
