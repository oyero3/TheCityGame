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
        private const int CITY_POINTS = 2;
        private int CurrentRerolls;
        private int Turn;
        private List<Die> Dice;
        bool isGameOver = false;

        public GameManager()
        {
            Random = new Random();
            Players = new List<Player>();
            Shop = new Shop();
            CityOccupant = null;
            CurrentPlayer = new Player("");
            CurrentRerolls = new int();
            Dice = new List<Die>();
            Turn = 1;
        }

        public void StartGame(int playerCount) 
        {

            InitializePlayers(playerCount);
            InitializeDice();
            DeterminePlayerOrder();

            do
            {
                foreach (var player in Players)
                {
                    StartNewTurn(player);
                    Shop.ShopPhase(Shop, Shop.AvaliableCards);
                    RollPhase();
                    ResolveDice();            
                    CheckWinConditions();
                }

                Turn++;
            } 
            while (!isGameOver);
        }

        private void InitializePlayers(int playerCount) 
        {
            // Initialize players
            for (int i = 0; i < playerCount; i++)
            {
                Console.Write($"Player [{i+1}] enter your name:");
                string? playername = Console.ReadLine();

                while (playername == null || playername == "")
                {
                    Console.WriteLine("Enter a valid name:");
                    playername = Console.ReadLine();
                }          

                Players.Add(new Player(playername));
                 Console.WriteLine("\n");
           }
        }

        private void InitializeDice()
        {
            for (int i = 0; i< MAX_DICE; i++)
            {
                var die = new Die();
                die.DieId = i+1;
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
                    roll += Random.Next(1,7);
                    total += roll;
                }

                playerRolls.Add(player, total);
                Console.WriteLine($"{player.Name}'s roll: {roll}\n");
            }
            Players = playerRolls.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
        }

        public void StartNewTurn(Player player)
        {
            CurrentRerolls = 0;
            CurrentPlayer = player;
            if (CurrentPlayer == CityOccupant) { CurrentPlayer.AddScore(2); }
            Console.WriteLine($"It is [{player.Name}'s] turn.");
            Console.WriteLine($"[{player.Name}] turn [{Turn}].");
        }


        public void RollPhase()
        {
            List<Die> TempRolls = new List<Die>();

            Console.WriteLine("Press space to roll:");
            Console.ReadKey();

            // First Roll
            foreach (var die in Dice) 
            {
                die.UnlockDie();
                die.Roll();
                TempRolls.Add(die);
                die.PrintDie();
            }

            // Reroll
            do 
            {
                Console.WriteLine($"\nWhich dice do you want to reroll?");
                string? userInput = Console.ReadLine();

                if (userInput == null || userInput == "None")
                {
                    Console.WriteLine("User is not rolling any more dice.");
                } 
                else
                {
                    List<int> diceToReroll = userInput.Split(' ').Select(int.Parse).ToList();
                    Console.Write($"Rerolling: ");
                    Console.WriteLine(string.Join(" ", diceToReroll));
                    for(int i = 0; i < diceToReroll.Count; i++)
                    {
                        int d = diceToReroll[i] -1;
                        TempRolls[d] = Dice[d].Reroll(CurrentRerolls);
                    }
                    PrintDice(TempRolls);
                    CurrentRerolls ++;
                }             
            } 
            while (CurrentRerolls < MAX_REROLLS);
            
            Dice = TempRolls.ToList();
            PrintDice(Dice);
            foreach (var die in Dice)
            {
                die.LockDie();
            }
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

            if (ones == 3) { scoring += 1; }
            if (ones == 4 || twos == 3) { scoring += 2; }
            if (ones == 5 || twos == 4 || threes == 3) { scoring +=3; }
            if (ones == 6 || twos == 5 || threes == 4) { scoring +=4; }
            if (twos == 6 || threes == 5) { scoring += 5; }
            if (threes == 6) { scoring += 6; }

            player.Heal(healing);
            player.AddMoney(money);
            player.AddScore(scoring);

            if (attacks > 0) 
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
            isGameOver = true;
        }

        private void PrintDice(List<Die> dice)
        {
            foreach (var die in dice)
            {
                Console.Write($"[{die.DieId} - {die.Value}] ");
            }
            Console.WriteLine("\n");
        }
    }
}