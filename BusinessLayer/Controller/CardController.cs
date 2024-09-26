﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MTCG.BusinessLayer.Controller
{
    internal class CardController
    {

        private static CardController _instance;

        public static CardController Instance => _instance ?? (_instance = new CardController());

        private CardController(){}

        public List<Card> GetCards(int count)
        {
            var cards = new List<Card>();

            for (var i = 0; i < count; i++)
            {
                cards.Add(GenerateCard());
            }

            return cards;
        }

        private static Card GenerateCard()
        {
            var cardType = new Random().Next(2);
            var damage = new Random().Next(200);

            if (cardType == 0)
            {
                return new SpellCard(damage, GetRandomElement());
            }
            
            return new MonsterCard(damage, GetRandomElement(), GetRandomMonsterType());
            
        }

        private static ElementType GetRandomElement()
        {
            var elements = (ElementType[])Enum.GetValues(typeof(ElementType));
            return elements[new Random().Next(elements.Length)];
        }

        private static MonsterType GetRandomMonsterType()
        {
            var monsterTypes = (MonsterType[])Enum.GetValues(typeof(MonsterType));
            return monsterTypes[new Random().Next(monsterTypes.Length)];
        }



    }
}
