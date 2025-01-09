using MTCG.BusinessLayer.Interface;
using MTCG.BusinessLayer.Model.Card;
using System;
using System.Collections.Generic;

namespace MTCG.BusinessLayer.Controller
{
    public class CardController : ICardController
    {

        private static ICardController _instance;

        public static ICardController Instance
        {
            get => _instance ??= new CardController();
            set => _instance = value;
        } 

        private CardController() { }

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
                random ??= new Random();
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
