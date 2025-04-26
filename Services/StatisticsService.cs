using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Memory.Models;

namespace Memory.Services
{
    public static class StatisticsService
    {
        private static readonly string filePath = "Data/statistics.json";

        public static List<UserStatistics> LoadStatistics()
        {
            //incarca statistici daca exista
            if (!File.Exists(filePath))
                return new List<UserStatistics>();

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<UserStatistics>>(json) ?? new List<UserStatistics>();
        }

        public static void SaveStatistics(List<UserStatistics> stats)
        {
            //scrie json ul in stats

            Directory.CreateDirectory("Data");
            var json = JsonSerializer.Serialize(stats, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public static void AddGameResult(string username, bool won)
        {
            //statistici pt fiecare player
            var stats = LoadStatistics();
            var userStats = stats.FirstOrDefault(s => s.Username == username);

            if (userStats == null)
            {
                userStats = new UserStatistics { Username = username }; 
                stats.Add(userStats);
            }

            userStats.GamesPlayed++;

            if (won)
                userStats.GamesWon++;

            SaveStatistics(stats);
        }


        public static void DeleteStatisticsForUser(string username)
        {
            //elimina stats
            var stats = LoadStatistics();
            stats.RemoveAll(s => s.Username == username);
            SaveStatistics(stats);
        }
    }
}
