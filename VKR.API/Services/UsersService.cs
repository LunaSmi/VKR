using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VKR.API.Models.Attach;
using VKR.API.Models.User;
using VKR.DAL;
using VKR.DAL.Entities;

namespace VKR.API.Services
{
    public class UsersService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        private Func<User, string?>? _linkGenerator;
        public void SetLinkGenerator(Func<User, string?> linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public UsersService(DataContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Guid> CreateUser(CreateUserModel createUserModel)
        {
            var dbuser = _mapper.Map<VKR.DAL.Entities.User>(createUserModel);
            var user = await _context.Users.AddAsync(dbuser);
            await _context.SaveChangesAsync();
            return user.Entity.Id;
        }

        public async Task<bool> CheckUserExistAsync(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task AddAvatarToUser(Guid userId, MetadataModel meta, string filePath)
        {
            var user = await _context.Users.Include(u => u.Avatar).FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                var avatar = new Avatar
                {
                    Author = user,
                    AuthorId = user.Id,
                    MimeType = meta.MimeType,
                    Name = meta.Name,
                    Size = meta.Size,
                    FilePath = filePath,
                };
                user.Avatar = avatar;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<AttachModel> GetUserAvatar(Guid userId)
        {
            var user = await GetUserById(userId);
            var attach = _mapper.Map<AttachModel>(user.Avatar);
            return attach;
        }
        // not used
        public async Task Delete(Guid userId)
        {
            var dbUser = await GetUserById(userId);
            if (dbUser != null)
            {
                _context.Users.Remove(dbUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserAvatarModel>> GetUsers()
        {
            return (await _context.Users.AsNoTracking().Include(x => x.Avatar).ToListAsync())
                 .Select(x => _mapper.Map<User, UserAvatarModel>(x, o => o.AfterMap(FixAvatar)));
        }

        public async Task<UserAvatarModel> GetUser(Guid userId)
        {
            return _mapper.Map<User, UserAvatarModel>(await GetUserById(userId), o => o.AfterMap(FixAvatar));
        }


        private async Task<VKR.DAL.Entities.User> GetUserById(Guid userId)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User Not Found");
            }
            return user;
        }

        private void FixAvatar(User user, UserAvatarModel userAvatarModel)
        {
            userAvatarModel.AvatarLink = user.Avatar == null ? null : _linkGenerator?.Invoke(user);
        }
    }
}
