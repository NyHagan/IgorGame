using Microsoft.AspNetCore.Mvc;
using rpsdyna.Models;
using System;

namespace rpsdyna.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Start()
        {
            HttpContext.Session.Clear();
            return View();
        }

        public IActionResult Backstory()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("PlayerHealth") == null)
            {
                HttpContext.Session.SetInt32("PlayerHealth", 50);
            }
            if (HttpContext.Session.GetInt32("ComputerHealth") == null)
            {
                HttpContext.Session.SetInt32("ComputerHealth", 50);
            }

            var gameModel = new GameModel
            {
                PlayerHealth = HttpContext.Session.GetInt32("PlayerHealth").Value,
                ComputerHealth = HttpContext.Session.GetInt32("ComputerHealth").Value
            };

            return View(gameModel);
        }

        [HttpPost]
        public IActionResult Play(string playerChoice, string bluffChoice = null, bool bluffAnnounced = false)
        {
            var gameModel = new GameModel
            {
                PlayerHealth = HttpContext.Session.GetInt32("PlayerHealth").Value,
                ComputerHealth = HttpContext.Session.GetInt32("ComputerHealth").Value
            };

            var random = new Random();
            string[] choices = { "Rock", "Paper", "Scissors" };

            if (bluffAnnounced && !string.IsNullOrEmpty(bluffChoice))
            {
                // Player has announced a bluff
                gameModel.IsBluffing = true;
                gameModel.BluffChoice = bluffChoice;
                gameModel.BluffAnnounced = true;

                // IGOR's response to the bluff
                if (random.Next(2) == 0) // 50% chance
                {
                    // Choose the move that beats the bluff
                    if (bluffChoice == "Rock")
                    {
                        gameModel.ComputerChoice = "Paper";
                    }
                    else if (bluffChoice == "Paper")
                    {
                        gameModel.ComputerChoice = "Scissors";
                    }
                    else if (bluffChoice == "Scissors")
                    {
                        gameModel.ComputerChoice = "Rock";
                    }
                }
                else
                {
                    // Choose any other move
                    gameModel.ComputerChoice = choices[random.Next(choices.Length)];
                }

                // We do not set PlayerChoice yet, as the player hasn't made the actual choice
            }
            else
            {
                // Normal game flow after bluff has been announced
                gameModel.PlayerChoice = playerChoice;

                // If IGOR's move was determined by bluff
                if (gameModel.IsBluffing)
                {
                    // Do nothing, IGOR's move is already set
                }
                else
                {
                    // Normal IGOR choice
                    gameModel.ComputerChoice = choices[random.Next(choices.Length)];
                }

                // Determine the result and adjust health points
                if (gameModel.PlayerChoice == gameModel.ComputerChoice)
                {
                    gameModel.Result = "It's a tie!";
                }
                else if ((gameModel.PlayerChoice == "Rock" && gameModel.ComputerChoice == "Scissors") ||
                         (gameModel.PlayerChoice == "Paper" && gameModel.ComputerChoice == "Rock") ||
                         (gameModel.PlayerChoice == "Scissors" && gameModel.ComputerChoice == "Paper"))
                {
                    gameModel.Result = "You win this round!";
                    gameModel.ComputerHealth -= 10;
                }
                else
                {
                    gameModel.Result = "IGOR wins this round!";
                    gameModel.PlayerHealth -= 10;
                }

                if (gameModel.PlayerHealth <= 0)
                {
                    gameModel.Result = "You Lost to IGOR!\r\n You're not going Anywhere";
                    gameModel.GameOver = true;
                }
                else if (gameModel.ComputerHealth <= 0)
                {
                    gameModel.Result = "You beat IGOR!\r\n You are free to Leave";
                    gameModel.GameOver = true;
                }

                // Store updated health points in session
                HttpContext.Session.SetInt32("PlayerHealth", gameModel.PlayerHealth);
                HttpContext.Session.SetInt32("ComputerHealth", gameModel.ComputerHealth);

                // Reset bluff-related properties after a round is completed
                gameModel.IsBluffing = false;
                gameModel.BluffAnnounced = false;
                gameModel.BluffChoice = null;
            }

            return PartialView("_GameResult", gameModel);
        }
    }
}