using Mapster;
using Library.Core.DTO;
using Library.Core.Model;
using Library.Core.Repository;

namespace Library.Core.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        this._userRepository = userRepository;
    }
    public UserProfileDto? GetUserById(int id)
    {
        var user = _userRepository.GetAllUsers().FirstOrDefault(user => user.Id == id);
        if(user is null)
            return null;
        
        var userToReturn = user.Adapt<UserProfileDto>(); 
        
        return userToReturn;
    }

    public List<UserProfileDto> GetUsers()
    {
        var users =_userRepository.GetAllUsers();
        var usersToReturn = users.Adapt<List<UserProfileDto>>();
        return users == null ? [] : usersToReturn;
    }

    public  UserProfileDto RemoveUser(int id)
    {
        var users = _userRepository.GetAllUsers();
        var user = users.FirstOrDefault(user => user.Id == id);
        if(user is null)
            return null!;

        var userToDelete = user.Adapt<UserProfileDto>();
        users.Remove(user);

        _userRepository.SaveUsers(users);
        return userToDelete!;
    }

    public UserProfileDto UpdateUser(int id, UpdateUserRoleDto updateUserRoleDto)
    {
        var users = _userRepository.GetAllUsers();
        var user = users.FirstOrDefault(user => user.Id == id);
        if(user is null)
            return null!;

        var userToUpdate = user.Adapt<UserProfileDto>();
        user!.Role = updateUserRoleDto.role;
        _userRepository.SaveUsers(users);
        return userToUpdate;
    }

    public UserProfileDto AddUser(CreateUserDto createUserDto)
    {
        var users = _userRepository.GetAllUsers();
        var user = new User
        {
            Name = createUserDto.Name,
            Id = users.Max(user => user.Id) + 1,
            Role = Role.User,
            Books = new List<Book>()
        };
        var userToAdd = user.Adapt<UserProfileDto>(); 
        users.Add(user);
        _userRepository.SaveUsers(users);
        
        return userToAdd ;
    }
}