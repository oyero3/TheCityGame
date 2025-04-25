using System;
using System.Collections.Generic;
using TheCityStrategyGame.Model.Cards;

namespace TheCityStrategyGame.Model 
{
    public class Shop
    {
        private CardDeck ActiveDeck;
        public List<Card> AvaliableCards;
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

        public void BuyCard(Player player, int cardIndex)
        {
            var card = this.AvaliableCards[cardIndex];

            if (player.Money >= card.Cost)
            {
                player.SpendMoney(card.Cost);
                card.AddCard(player, player.Cards, card);     
                card.DiscardCard(player, AvaliableCards, card);
            }
            else
            {
                throw new InvalidOperationException("Not enough money to buy this card");
            }
        }

    }

    
}