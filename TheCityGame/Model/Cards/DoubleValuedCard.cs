namespace TheCityStrategyGame.Model.Cards
{
    public class DoubleValueCard : Card
    {
        private Die.DieValue targetValue;
        public DoubleValueCard()
        {
            Name = "Double Value 3's";
            Description = $"Main Effect: Double points when rolling a set of 3s";
            Cost = 5;
        }

        public override void ModifySetScore(Die.DieValue setValue, int score)
        {
            if(setValue == Die.DieValue.Three)
            {
                score = 2 * score;
            }
        }

    }
}