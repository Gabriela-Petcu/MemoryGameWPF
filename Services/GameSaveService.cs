using Memory.Models;
using System.IO;
using System.Text.Json;

namespace Memory.Services
{
    public static class GameSaveService
    {
        private static string SaveDirectory => "Data/Saves";

        public static void SaveGame(SavedGameModel game)
        {
            Directory.CreateDirectory(SaveDirectory); 

            string filePath = Path.Combine(SaveDirectory, $"{game.Username}_save.json");
            string json = JsonSerializer.Serialize(game, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(filePath, json);
        }



        public static SavedGameModel LoadGame(string username)
        {
            string filePath = Path.Combine(SaveDirectory, $"{username}_save.json");
            if (!File.Exists(filePath))
                return null;

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<SavedGameModel>(json);
        }

        public static void DeleteSavedGame(string username)
        {
            string filePath = Path.Combine(SaveDirectory, $"{username}_save.json");
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
