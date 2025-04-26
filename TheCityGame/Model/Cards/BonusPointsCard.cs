namespace TheCityStrategyGame.Model.Cards
{
    public class BonusPointsCard : Card
    {
        private int bonusPoints = 4;

        public BonusPointsCard() 
        {
            Name = "Bonus Points";
            Description = "+ 4 Points";
            Cost = 3;
        }

        public override void OnAcquired(Player owner)
        {
            owner.AddScore(bonusPoints);
            owner.Cards.Remove(this);
        }
    }
}