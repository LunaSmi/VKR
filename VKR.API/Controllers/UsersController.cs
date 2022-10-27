using AutoMapper;
using AutoMapper.QueryableExtensions;
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
           await _usersService.CreateUser(createUserModel);
        }

        [HttpGet]
        public async Task<List<UserModel>> GetUsers()
        {
           return await _usersService.GetUsers();
        }

    }
}
