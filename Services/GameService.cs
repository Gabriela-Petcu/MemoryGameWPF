using Memory.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;


namespace Memory.Services
{
    public class GameService
    {
        public List<TileModel> GenerateTiles(string category, int rows, int cols)
        {
            //nr perechu
            int pairCount = (rows * cols) / 2;

            //calea completa
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", category);
           
            //listeaza imaginile din folderul respectiv
            var imageFiles = Directory.GetFiles(folderPath, "*.jpg").ToList();

            var random = new Random();

            //amesteca si aege nr egal de imagini cu nr de perechi
            var selected = imageFiles.OrderBy(x => random.Next()).Take(pairCount).ToList();

            //genereaza perechile
            var tiles = new List<TileModel>();
            int id = 1;


            foreach (var img in selected)
            {
                var relativePath = img.Replace(AppDomain.CurrentDomain.BaseDirectory, "").Replace("\\", "/");
                var fullPath = $"pack://siteoforigin:,,,/{relativePath}";

                tiles.Add(new TileModel { ImagePath = fullPath, Id = id });
                tiles.Add(new TileModel { ImagePath = fullPath, Id = id });
                id++;
            }

            return tiles.OrderBy(x => random.Next()).ToList();

        }

        public void SaveGame(GameStateModel state)
        {
            Directory.CreateDirectory("SavedGames");
            string path = $"SavedGames/{state.Username}_save.json";
            string json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }

        public GameStateModel LoadGame(string username)
        {
            //incarca jocul pt user daca exista fisierul
            string path = $"SavedGames/{username}_save.json";
            if (!File.Exists(path)) return null;

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<GameStateModel>(json);
        }
    }
}
