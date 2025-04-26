namespace Memory.Models
{
    public class SavedGameModel
    {
        //salvare in json
        public string Username { get; set; }
        public string Category { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<TileModel> Tiles { get; set; }
        public int TimeRemaining { get; set; }
        public int ElapsedSeconds { get; set; }
    }
}
