namespace rpsdyna.Models
{
    public class GameModel
    {
        public string PlayerChoice { get; set; }
        public string ComputerChoice { get; set; }
        public string BluffChoice { get; set; } // Property for player's bluff choice
        public string Result { get; set; }
        public int PlayerHealth { get; set; } = 100;
        public int ComputerHealth { get; set; } = 100;
        public bool GameOver { get; set; } = false;
        public bool IsBluffing { get; set; } = false; // Tracks if the player is bluffing
        public bool BluffAnnounced { get; set; } = false; // Tracks if the bluff has been announced
    }
}