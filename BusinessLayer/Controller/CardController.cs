using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MTCG.BusinessLayer.Interface;

namespace MTCG.BusinessLayer.Controller
{
    internal class CardController
    {

        private static CardController _instance;

        public static CardController Instance => _instance ?? (_instance = new CardController());

        private CardController(){}

        private static Random random;
        private static readonly object syncObj = new object();
        private static void InitRandomNumber(int seed)
        {
            random = new Random(seed);
        }
        private static int GenerateRandomNumber(int max)
        {
            lock (syncObj)
            {
                if (random == null)
                    random = new Random(); // Or exception...
                return random.Next(max);
            }
        }

        public List<ICard> GetCards(int count)
        {
            var cards = new List<ICard>();

            for (var i = 0; i < count; i++)
            {
                cards.Add(GenerateCard());
            }

            return cards;
        }

        private static Card GenerateCard()
        {
            var cardType = GenerateRandomNumber(2);
            var damage = GenerateRandomNumber(200);

            if (cardType == 0)
            {
                return new SpellCard(damage, GetRandomElement());
            }
            
            return new MonsterCard(damage, GetRandomElement(), GetRandomMonsterType());
            
        }

        private static ElementType GetRandomElement()
        {
            var elements = (ElementType[])Enum.GetValues(typeof(ElementType));
            return elements[GenerateRandomNumber(elements.Length)];
        }

        private static MonsterType GetRandomMonsterType()
        {
            var monsterTypes = (MonsterType[])Enum.GetValues(typeof(MonsterType));
            return monsterTypes[GenerateRandomNumber(monsterTypes.Length)];
        }



    }
}
