using Library.Core.DTO;
using Library.Core.Model;

namespace Library.Core.Service;

public interface IUserService
{
    /*
        the services that user needs in the program Create UserAccount and Update Their Data 
        and then Read their Accounts and delete their Accounts   
    */
    UserProfileDto AddUser(CreateUserDto createUserDto);
    List<UserProfileDto> GetUsers();
    UserProfileDto? GetUserById(int id);
    UserProfileDto? RemoveUser(int id);
    UserProfileDto? UpdateUser(int id, UpdateUserRoleDto updateUserRoleDto);
}
