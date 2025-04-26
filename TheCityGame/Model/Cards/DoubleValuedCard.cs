namespace TheCityStrategyGame.Model.Cards
{
    public class DoubleValueCard : Card
    {
        private Die.DieValue targetValue = Die.DieValue.Three;
        public DoubleValueCard()
        {
            Name = "Double Value 3's";
            Description = $"Main Effect: Double points when rolling a set of 3s";
            Cost = 5;
        }

        public void ModifySetScore( int score)
        {
            if(targetValue == Die.DieValue.Three)
            {
                score = 2 * score;
            }
        }

    }
}