using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VKR.API.Models;
using VKR.DAL;

namespace VKR.API.Services
{
    public class UsersService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;


        public UsersService(
            DataContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task CreateUser(CreateUserModel createUserModel)
        {
            var dbuser = _mapper.Map<VKR.DAL.Entities.User>(createUserModel);
            await _context.Users.AddAsync(dbuser);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserModel>> GetUsers()
        {
            return await _context.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

    }
}
