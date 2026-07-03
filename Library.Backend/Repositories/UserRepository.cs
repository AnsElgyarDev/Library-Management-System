using System.Text.Json;
using Library.Core.Model;

namespace Library.Core.Repository;

class UserRepository : IUserRepository 
{
    public string FilePath = @"D:\Desktop\Library Management System\Library.Backend\users.Json";

    public List<User> GetAllUsers()
    {
        if(!File.Exists(FilePath))
        {
            return new List<User>();
        }

        string StringFile = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<User>>(StringFile) ?? new List<User>();
    }

    public void SaveUsers(List<User> Users)
    {
        string JsonString = JsonSerializer.Serialize(Users, new JsonSerializerOptions{ WriteIndented = true });

        File.WriteAllText(FilePath, JsonString);
    }
}