using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VKR.API.Models;
using VKR.API.Services;
using VKR.DAL;

namespace VKR.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersService _usersService;

        public UsersController(
            UsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost]
        public async Task CreateUser(CreateUserModel createUserModel)
        {
            if (await _usersService.CheckUserExistAsync(createUserModel.Email))
                throw new Exception("user is exist");
            await _usersService.CreateUser(createUserModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<UserModel> GetCurrentUser()
        {
            var userIdString = User.Claims.FirstOrDefault(c=>c.Type=="Id")?.Value;
            if(Guid.TryParse(userIdString, out var userId))
            {
                return await _usersService.GetUser(userId);
            }
            else
            {
                throw new Exception("You are not authorized");
            }
        }


        [HttpGet]
        [Authorize]
        public async Task<List<UserModel>> GetUsers()
        {
           return await _usersService.GetUsers();
        }

    }
}
