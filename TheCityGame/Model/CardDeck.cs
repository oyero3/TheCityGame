using TheCityStrategyGame.Model.Cards;

namespace TheCityStrategyGame.Model 
{
    public class CardDeck
    {
        public List<Card> Cards;
        public CardDeck()
        {
            Cards = new List<Card>();
            InitializeCards();
        }

        private void InitializeCards()
        {
            Cards.Add(new DoubleValueCard());
            Cards.Add(new BonusPointsCard());
            // In a real implementation, we'd add many more card types here  
        }

        public void AddCard( Card card)
        {
            Cards.Add(card);
        }
        public void RemoveCard(Card card)
        {
            Cards.Remove(card);
        }
    }
}