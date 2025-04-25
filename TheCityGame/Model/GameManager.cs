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
        private int Turn;
        private List<Die> Dice;
        bool isGameOver = false;

        public GameManager(int playerCount)
        {
            Random = new Random();
            Players = new List<Player>();
            Shop = new Shop();
            CityOccupant = null;
            CurrentPlayer = new Player();
            CurrentRerolls = new int();
            Dice = new List<Die>();
            Turn = 1;

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

            do
            {
                foreach (var player in Players)
                {
                    StartNewTurn(player);
                    ShopPhase();
                    RollPhase();
                    ResolveDice();            
                    CheckWinConditions();
                    Turn++;
                }
            } 
            while (!isGameOver);
        }

        private void InitializeDice()
        {
            for (int i = 0; i< MAX_DICE; i++)
            {
                var die = new Die();
                die.DieId = i;
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
            Players = playerRolls.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
        }

        public void StartNewTurn(Player player)
        {
            CurrentRerolls = 0;
            CurrentPlayer = player;
            Console.WriteLine($"It is [{player.Name}'s] turn.");
            Console.WriteLine($"[{player.Name}] turn [{Turn}].");
        }

        public void ShopPhase()
        {
            Shop.Refresh();
            foreach (var card in Shop.AvaliableCards)
            {
                Console.WriteLine($"Card: [{card.Name}] - Cost: [${card.Cost}] - Description: [{card.Description}]");
            }
        }
        public void RollPhase()
        {
            List<Die> TempRolls = new List<Die>();

            // First Roll
            foreach (var die in Dice) 
            {
                die.UnlockDie();
                die.Roll();
                TempRolls.Add(die);
            }

            // Reroll
            do 
            {
                PrintDice(TempRolls);
                Console.WriteLine($"Which dice do you want to reroll?");
                string? userInput = Console.ReadLine();

                if (userInput == null || userInput == "None")
                {
                    Console.WriteLine("User is not rolling any more dice.");
                } 
                else
                {
                    List<int> diceToReroll = userInput.Split(',').Select(int.Parse).ToList();
                    for(int i = 0; i < diceToReroll.Count; i++)
                    {
                        int d = diceToReroll[i];
                        TempRolls[diceToReroll[d]] = RerollSelectedDie(Dice[diceToReroll[d]]);
                    }
                    CurrentRerolls ++;
                }             
            } 
            while (CurrentRerolls < MAX_REROLLS);
            
            Dice = TempRolls.OrderBy(x=> x.Value).ToList();
            foreach (var die in Dice)
            {
                die.LockDie();
            }
        }
        public Die RerollSelectedDie(Die die)
        {
            if (CurrentRerolls < MAX_REROLLS)
            {
                die.Roll();
            }
            return die;
        }

        public void ResolveDice()
        {
            var player = CurrentPlayer;
            int scoring = 0;
            int healing = 0;
            int attacks = 0;
            int money = 0;
            int ones = 0;
            int twos = 0;
            int threes = 0;
            List<Die> TempDice = Dice;

            foreach (var die in TempDice)
            {
                switch (die.Value)
                {
                    case Die.DieValue.Attack: attacks++;break;
                    case Die.DieValue.Heart: healing++;break;
                    case Die.DieValue.Money: money++;break;
                    case Die.DieValue.One: ones++; break;
                    case Die.DieValue.Two: twos++; break;
                    case Die.DieValue.Three: threes++; break;
                }             
            }

            if (ones == 3) { scoring =+ 1; }
            if (ones == 4 || twos == 3) { scoring =+ 2; }
            if (ones == 5 || twos == 4 || threes == 3) { scoring =+3; }
            if (ones == 6 || twos == 5 || threes == 4) { scoring =+4; }
            if (twos == 6 || threes == 5) { scoring += 5; }
            if (threes == 6) { scoring += 6; }

            player.Heal(healing);
            player.AddMoney(money);
            player.AddScore(scoring);

            if (attacks>0) 
            {
                ProcessAttacks(player, attacks);
            }
        }

        private void ProcessAttacks(Player attacker, int attacks)
        {
            // Handle city entrance if applicable
            if (CityOccupant == null && attacks > 0)
            {
                EnterCity(attacker);
            }
            else if (attacker == CityOccupant)
            {
                foreach (var defender in Players.Where(p => p != CityOccupant))
                {
                    defender.TakeDamage(attacks);
                }
            }
            else if (CityOccupant != null) 
            {
                CityOccupant.TakeDamage(attacks);
                Console.WriteLine($"Does [{CityOccupant.Name}] want to leave the city? (Y/N)");
                var input = Console.ReadKey();

                if (input.Key == ConsoleKey.Y)
                {
                    ExitCity(CityOccupant);
                    EnterCity(attacker);
                }
            }

        }

        private void EnterCity(Player player)
        {
            CityOccupant = player;
            player.AddScore(1);
        }

        private void ExitCity(Player player)
        {
            if (CityOccupant == player)
            {
                CityOccupant = null;
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

        public event Action<Player, WinReason> GameOver;

        private void OnGameOver(Player winner, WinReason reason)
        {
            GameOver?.Invoke(winner, reason);
        }

        private void PrintDice(List<Die> dice)
        {
            foreach (var die in dice)
            {
                Console.WriteLine($"Die Number [{die.DieId}] Value = [{die.Value}]");
            }
        }
    }
}