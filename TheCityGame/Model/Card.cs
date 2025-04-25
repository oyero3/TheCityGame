using System;
using TheCityStrategyGame.Model;

namespace TheCityStrategyGame.Model
{
    public class Card 
    {        
        #region Fields
        public string Name {get; set;}
        public string Description {get; set;}
        public int Cost {get; set;}
        #endregion

        #region Methods
        protected Card()
        {
            Name = "";
            Description = "";
            Cost = 0;
        }

        public void AddCard(Player player, List<Card> cards ,Card card)
        {
            cards.Add(card);
            card.OnAcquired(player);
        }
        public void UseCard(Player player, List<Card> cards ,Card card)
        {
            card.OnUsed(player);
        }
        public void DiscardCard(Player player, List<Card> cards ,Card card)
        {
            cards.Remove(card);
            card.OnDiscarded(player);
        }
        
        public virtual void Effect() {}
        public virtual void OnAcquired(Player owner) { }
        public virtual void OnUsed(Player owner) { }
        public virtual void OnDiscarded(Player owner) { }
        public virtual void ModifyScore(Player owner) { }
        public virtual void ModifySetScore(Die.DieValue setValue, int score) { }
        public virtual void ModifyDamage(int damage) { } 
        
        #endregion
    }
}