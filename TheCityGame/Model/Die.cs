using System;

namespace TheCityStrategyGame.Model
{
    public class Die
    {
        #region Fields

        private static Random Random = new ();
        public int DieId {get; set;}
        public DieValue Value {get; set; }
        public bool IsLocked { get; set; }
        private static int MAX_REROLLS = 5;
        #endregion

        #region Enums
        public enum DieValue
        {
            One = 1,
            Two = 2,
            Three = 3,
            Heart = 4,
            Attack = 5,
            Money = 6,
        }
        #endregion

        #region Methods
        public Die()
        {
            DieId = 0;
            IsLocked = false;
        }

        public void Roll()
        {
            if (!IsLocked)
            {
                int roll = Random.Next(1, 7);
                Value = (DieValue)roll;
            }
        }
        public Die Reroll(int CurrentRerolls)
        {
            if (CurrentRerolls < MAX_REROLLS)
            {
                this.Roll();
            }
            return this;
        }
        public void LockDie()
        {
            IsLocked = true; 
        }

        public void UnlockDie()
        {
            IsLocked = false;
        }

        public void PrintDie()
        {
            Console.Write($"[{this.DieId} - {this.Value}] ");
        }
        #endregion
        
    }
}