using Practice.Model;

namespace Practice.Repository;

public interface IUserRepository
{
    List<User> GetAllUsers();
    void SaveUsers(List<User> Users);
}