using Memory.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Memory.Services
{
    public class UserService
    {
        private const string FilePath = "users.json";

        public List<UserModel> LoadUsers()
        {
            //ia din fisier si transf in lista
            if (!File.Exists(FilePath)) return new List<UserModel>();
            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<UserModel>>(json);
        }

        public void SaveUsers(List<UserModel> users)
        {
            //invers
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}
