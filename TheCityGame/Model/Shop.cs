using System;
using System.Collections.Generic;
using TheCityStrategyGame.Model.Cards;

namespace TheCityStrategyGame.Model 
{
    public class Shop
    {
        private CardDeck ActiveDeck;
        private List<Card> AvaliableCards;
        private List<Card> BoughtCards;
        private const int SHOP_SIZE = 5;

        public Shop()
        {
            ActiveDeck = new CardDeck();
            AvaliableCards = new List<Card>();
            BoughtCards = new List<Card>();
            Refresh();
        }

        public void Refresh()
        {
            AvaliableCards.Clear();
            for (int i = 0; i < SHOP_SIZE; i++)
            {
                Card card = ActiveDeck.Cards[i];
                if (card != null)
                {
                    AvaliableCards.Add(card);
                    ActiveDeck.RemoveCard(card);
                }
            }
        }

    }

    
}