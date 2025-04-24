using System;
using System.Linq;
using System.Collections.Generic;
using TheCityStrategyGame.Model.Cards;
using System.Xml.Serialization;

namespace TheCityStrategyGame.Model
{
    public class GameManager
    {
        private List<Player> Players;
        private Shop Shop;
        private Player? CityOccupant;
        private Player CurrentPlayer;
        private Random Random;
        private const int WINNING_SCORE = 20;
        private const int MAX_REROLLS = 2;
        private const int MAX_DICE = 6;
        private int CurrentRerolls;
        private List<Die> Dice;

        public GameManager(int playerCount)
        {
            Random = new Random();
            Players = new List<Player>();
            Shop = new Shop();
            CityOccupant = null;
            CurrentPlayer = new Player();
            CurrentRerolls = new int();
            Dice = new List<Die>();
            GameOver = false;

            // Initialize players
            for (int i = 0; i < playerCount; i++)
            {
                Players.Add(new Player());
            }

            // Initialize the dice
            for (int i = 0; i < MAX_DICE; i++)
            {
                Dice.Add(new Die());
            }

            InitializeDice();
            DeterminePlayerOrder();

            do{
                foreach (var player in Players)
                {
                    StartNewTurn();
                    RollPhase();

                }
            }while (true);
        }

        private void InitializeDice()
        {
            for (int i = 0; i< MAX_DICE; i++)
            {
                var die = new Die();
                Dice.Add(die);
            }
        }
        private void DeterminePlayerOrder()
        {
            // Roll dice to determine player order
            Dictionary<Player, int> playerRolls = new Dictionary<Player, int>();
            foreach (var player in Players)
            {
                int roll = 0;
                int total = 0;
                foreach (var die in Dice) 
                {
                    roll =+ Random.Next(1,7);
                    total =+ roll;
                }

                playerRolls.Add(player, total);
            }
            // Sort Players by roll
            Players = playerRolls.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
        }



        public void StartNewTurn()
        {
            CurrentRerolls = 0;
            foreach (var die in Dice)
            {
                die.Roll();
            }

            RerollSelectedDice();
        }

        public void RollPhase()
        {
            bool playerIsRerolling = true;
            List<Die.DieValue> FinalRolls = new List<Die.DieValue>();
            List<Die.DieValue> TempRolls = new List<Die.DieValue>();

            foreach(var die in Dice) 
            {
                die.Roll();
                TempRolls.Add(die.Value);            
            }

            
        }
        public void RerollSelectedDice()
        {
            if (CurrentRerolls < MAX_REROLLS)
            {
                foreach (var die in Dice)
                {
                    if (!die.IsLocked)
                    {
                        die.Roll();
                    }
                }
                CurrentRerolls++;
            }
            else
            {
                foreach (var die in Dice)
                {
                    die.IsLocked = true;
                }

                throw new InvalidOperationException("No rerolls left for this turn.");
            }
        }

        public void ResolveDice()
        {
            var player = CurrentPlayer;
            int scoring = 0;
            int healing = 0;
            int attacks = 0;
            int money = 0;

            // Count dice results
            Dictionary<Die.DieValue, int> counts = new Dictionary<Die.DieValue, int>();
            foreach (Die.DieValue value in Enum.GetValues(typeof(Die.DieValue)))
            {
                counts[value] = 0;
            }

            foreach (var die in Dice)
            {
                counts[die.Value]++;
            }

            // Resolve number sets for scoring
            for (int i = 1; i <= 3; i++)
            {
                Die.DieValue value = (Die.DieValue)i;
                if (counts[value] >= 3)
                {
                    int setScore = i;
                    foreach(var card in player.Cards)
                    {
                        setScore = card.ModifySetScore(value, setScore);
                    }

                    scoring += setScore;
                }
            }   
            player.AddScore(scoring);

            // Resolve healing
            healing = counts[DieValue.Heart];
            player.Heal(healing);

            // Resolve attacks
            attacks = counts[DieValue.Attack];
            
            // Resolve money
            money = counts[DieValue.Money];
            player.AddMoney(money);

            // Handle city entrance if applicable
            if (cityOccupant == null && attacks > 0)
            {
                EnterCity(player);
            }

            // Apply attacks
            if (attacks>0) 
            {
                ProcessAttacks(player, attacks);
            }

            // Check for winning condition
            CheckWinConditions();

            NextPlayer();
        }

        private void ProcessAttacks(Player attacker, int attackCount)
        {
            if (attacker == cityOccupant)
            {
                foreach (var defender in Players.Where(p => p != cityOccupant))
                {
                    defender.TakeDamage(attackCount);
                }
            }
            else if (cityOccupant != null) 
            {
                cityOccupant.TakeDamage(attackCount);
                bool leavesCity = true;

                if (leavesCity)
                {
                    ExitCity(cityOccupant);
                    EnterCity(attacker);
                }
            }
        }

        private void EnterCity(Player player)
        {
            cityOccupant = player;
            player.AddScore(1);
        }

        private void ExitCity(Player player)
        {
            if (cityOccupant == player)
            {
                cityOccupant = null;
            }
        }

        public void BuyCard(int cardIndex)
        {
            var player = CurrentPlayer;
            var card = shop.AvailableCards[cardIndex];

            if (player.Money >= card.Cost)
            {
                player.SpendMoney(card.Cost);
                player.AddCard(card);
                shop.RemoveCard(cardIndex);
            }
            else
            {
                throw new InvalidOperationException("Not enough money to buy this card");
            }
        }

        private void CheckWinConditions()
        {
            foreach (var player in Players)
            {
                if (player.Score >= WINNING_SCORE)
                {
                    OnGameOver(player, WinReason.Score);
                    return;
                }

                if (player.Health <= 0)
                {
                    player.IsDead = true;
                }
            }

            // Check remaining Players
            var remainingPlayers = Players.Where(p => !p.IsDead).ToList();
            if (remainingPlayers.Count == 1)
            {
                OnGameOver(remainingPlayers[0], WinReason.LastStanding);
            }
        }

        private void NextPlayer()
        {
            do
            {
                currentPlayerIndex = (currentPlayerIndex +1) % Players.Count;
            } while (Players[currentPlayerIndex].IsDead);

            if (currentPlayerIndex == 0)
            {
                shop.Refresh();

                if (cityOccupant != null)
                {
                    cityOccupant.AddScore(2);
                }
            }
        }

        public event Action<Player, WinReason> GameOver;

        private void OnGameOver(Player winner, WinReason reason)
        {
            GameOver?.Invoke(winner, reason);
        }
    }
}