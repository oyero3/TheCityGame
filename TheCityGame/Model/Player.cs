using System;
using System.Collections.Generic;
using TheCityStrategyGame.Model.Cards;

namespace TheCityStrategyGame.Model
{
    public class Player
    {
        #region Constants
        private const int MAX_HEALTH = 10;
        private const int MAX_SCORE = 20;
        private const int MAX_MONEY = 100;
        private const int MAX_STRENGTH = 10;
        private const int MAX_CARDS = 5;
        #endregion

        #region Fields
        public string Name {get; }
        public int Health {get; private set; }
        public int MaxHealth {get; private set;}
        public int Score {get; private set;}
        public int Strength {get; private set;}
        public int Money {get; private set;}
        public readonly List<Card> Cards;
        public bool IsDead {get; set; }
        #endregion

        #region Player
        public Player(string name)
        {
            Name = name;
            Health = 10;
            MaxHealth = MAX_HEALTH;
            Score = 0;
            Money = 0;
            Strength = 1;
            IsDead = false;
            Cards = new List<Card>();
        }
        #endregion

        #region Methods
        public void Heal(int amount)
        {
            Health = Math.Min(Health + amount, MaxHealth);
            Console.WriteLine($"Player [{Name}] is healing to [{Health}/{MAX_HEALTH}] health.");
        }

        public void TakeDamage(int amount)
        {        
            Health -= amount;
            Console.WriteLine($"Player [{Name}] is taking [{amount}] damage.");
        }

        public void AddScore(int amount)
        {            
            Score += amount;
            Console.WriteLine($"Player [{Name}] gained [{amount}] points.");
        }

        public void AddMoney(int amount)
        {
            Money += amount;
            Console.WriteLine($"Player [{Name}] gained [${amount}].");
        }

        public void SpendMoney(int amount)
        {
            Money -= amount;
            Console.WriteLine($"Player [{Name}] spent [${amount}].");
        }

        public void Reset()
        {
            if (IsDead == true)
            {
                Health = MaxHealth;
                Score = 0;
                Money = 0;
                Strength = 1;
                IsDead = false;
                Cards.Clear();
            }
        }

        #endregion
    }
}