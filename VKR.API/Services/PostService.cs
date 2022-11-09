using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using VKR.API.Models;
using VKR.API.Models.Attach;
using VKR.API.Models.Post;
using VKR.API.Models.User;
using VKR.DAL;
using VKR.DAL.Entities;

namespace VKR.API.Services
{
    public class PostService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly UsersService _usersService;
        private Func<AttachModel, string?>? _linkContentGenerator;
        private Func<UserModel, string?>? _linkAvatarGenerator;
        public void SetLinkGenerator(Func<AttachModel, string?> linkContentGenerator, Func<UserModel, string?> linkAvatarGenerator)
        {
            _linkAvatarGenerator = linkAvatarGenerator;
            _linkContentGenerator = linkContentGenerator;
        }

        public PostService(DataContext context,
            IMapper mapper,
            UsersService usersService)
        {
            _context = context;
            _mapper = mapper;
            _usersService = usersService;
        }

        public async Task<Guid> CreatePost(CreatePostModel createPostModel, Guid ownerId)
        {
            var dbpost = _mapper.Map<VKR.DAL.Entities.Post>(createPostModel);
            dbpost.Created = DateTimeOffset.Now.UtcDateTime;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == ownerId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User Not Found");
            }
            else
            {
                dbpost.Owner = user;
                var post = await _context.Posts.AddAsync(dbpost);
                await _context.SaveChangesAsync();
                return post.Entity.Id;
            }
        }




        public async Task AddPhotosToPost(Guid postId, List<MetaDataModel> meta, string filePath)
        {
            var post = await _context.Posts.Include(u => u.Photos).FirstOrDefaultAsync(x => x.Id == postId);

            if (post != null)
            {
                var photos = new List<Photo>();

                foreach (var m in meta)
                {
                    photos.Add(new Photo
                    {
                        Owner = post.Owner,
                        MimeType = m.MimeType,
                        Name = m.Name,
                        Size = m.Size,
                        FilePath = filePath,
                    });
                }

                post.Photos = photos;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<AttachModel>> GetPostPhotos(Guid postId)
        {
            var attaches = new List<AttachModel>();
            var photos = await _context.Photos.Where(x => x.Id == postId).ToListAsync();

            foreach (var photo in photos)
            {
                attaches.Add(_mapper.Map<AttachModel>(photo));
            }
            return attaches;
        }




    }
}
