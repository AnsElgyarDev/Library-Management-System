using System.Net.Http.Headers;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Http.HttpResults;
using Practice.Common;
using Practice.DTO;
using Practice.Model;
using Practice.Service;

namespace Practice.Endpoints;
public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        // Get Endpoints
        app.MapGet("/Users", Results<NotFound<string>, Ok<List<UserProfileDto>>> (IUserService userService) =>
        {
            var users = userService.GetUsers();
            if(users is null)
                return TypedResults.NotFound("Not Found!");

            return TypedResults.Ok(users);
        });

        app.MapGet("/Users/{id}", Results<NotFound<string>, Ok<UserProfileDto>>(IUserService userService, int id) =>
        {
            var user = userService.GetUserById(id);
            if(user is null)
                return TypedResults.NotFound("Not Found!");
            
            return TypedResults.Ok(user);
        });

        // Post Endpoints
        app.MapPost("/Users", Results<NotFound<string>, Ok<UserProfileDto>>(IUserService userService, CreateUserDto createUserDto) =>
        {
            var userToAdd = userService.AddUser(createUserDto);
            if(userToAdd is null)
                return TypedResults.NotFound("Not Found!");
           
            return TypedResults.Ok(userToAdd);
        });

        // Delete Endpoints
        app.MapDelete("/Users/{id}", Results<NotFound<string>, Ok<UserProfileDto>> (IUserService userService, int id) =>
        {
            var userToDelete = userService.RemoveUser(id);
            if(userToDelete is null)
                return TypedResults.NotFound("Not Found!");
            return TypedResults.Ok(userToDelete);
        });

        // Update Endpoints
        app.MapPut("/Users/{id}", Results<NotFound<string>, Ok<UserProfileDto>> 
                  (IUserService userService, UpdateUserRoleDto userRoleDto, int id) =>
        {
            var userToUpdate = userService.UpdateUser(id, userRoleDto);
            if(userToUpdate is null)
                return TypedResults.NotFound("Not Found!");

            return TypedResults.Ok(userToUpdate);
        });
    }
}