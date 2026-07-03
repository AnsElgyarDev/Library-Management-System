using Library.Core.Model;

namespace Library.Core.Repository;

public interface IUserRepository
{
    List<User> GetAllUsers();
    void SaveUsers(List<User> Users);
}