using System;
using TheCityStrategyGame.Model;

namespace TheCityStrategyGame
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Write("Welcome to The City Strategy Game!");
                Console.Write("How many players? (2-4 recommended):");
                
                // Get player count input
                int playerCount;
                while (!int.TryParse(Console.ReadLine(), out playerCount) || playerCount < 1)
                {
                    Console.WriteLine("Please enter a valid number of players (1 or more):");
                }
                
                // Create and start the game
                GameManager gameManager = new GameManager();
                
                // Subscribe to game over event
                gameManager.GameOver += (winner, reason) => {
                    Console.WriteLine($"Game Over! {winner.Name} wins by {reason}!");
                    Console.WriteLine($"Final Score: {winner.Score}");
                };
                
                // Start the game with the specified number of players
                gameManager.StartGame(playerCount);
                
                Console.WriteLine("Thanks for playing! Press any key to exit.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}