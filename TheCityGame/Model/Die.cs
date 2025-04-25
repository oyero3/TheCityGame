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
        public void LockDie()
        {
            IsLocked = true; 
        }

        public void UnlockDie()
        {
            IsLocked = false;
        }
        #endregion
        
    }
}