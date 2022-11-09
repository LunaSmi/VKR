using AutoMapper;
using VKR.API.Models;
using VKR.DAL;

namespace VKR.API.Services
{
    public class CommentService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;


        public CommentService(DataContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //public async Task<Guid> AddComment(string comment)
        //{
        //    var dbuser = _mapper.Map<VKR.DAL.Entities.User>(createUserModel);

        //    var user = await _context.Users.AddAsync(dbuser);

        //    await _context.SaveChangesAsync();
        //    return user.Entity.Id;
        //}



    }
}
