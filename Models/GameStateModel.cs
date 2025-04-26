using System.Collections.Generic;

namespace Memory.Models
{
    public class GameStateModel
    {
        //starea curenta a jocului
        public string Username { get; set; }
        public string Category { get; set; }
        public int TimeLeft { get; set; }
        public List<TileModel> Tiles { get; set; }
    }
}
