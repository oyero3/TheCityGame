namespace TheCityStrategyGame.Model.Cards
{
    public class BonusPointsCard : Card
    {
        private int bonusPoints;

        public BonusPointsCard() 
        {
            Name = "Bonus Points";
            Description = "Roll 1 die and add that amount to your points. (Roll until you get a number)";
            Cost = 3;
        }

        public override void OnAcquired(Player owner)
        {
            owner.AddScore(bonusPoints);
            owner.Cards.Remove(this);
        }
    }
}