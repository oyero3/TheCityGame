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

        public virtual void OnAcquired(Player owner) { }
        public virtual void OnUsed(Player owner) { }
        public virtual void OnDiscarded(Player owner) { }
        public virtual void ModifyScore(Player owner) { }
        public virtual void ModifySetScore(Die.DieValue setValue, int score) { }
        public virtual void ModifyDamage(int damage) { } 
        
        #endregion
    }
}