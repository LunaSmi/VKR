using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VKR.API.Configs;
using VKR.API.Models.Attach;
using VKR.API.Models.Post;
using VKR.API.Models.User;
using VKR.DAL;
using VKR.DAL.Entities;

namespace VKR.API.Services
{
    public class PostsService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public PostsService(IMapper mapper, IOptions<AuthConfig> config, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }


        public async Task CreatePost(CreatePostRequest request)
        {
            var model = _mapper.Map<CreatePostModel>(request);
            model.Contents.ForEach(x =>
            {
                x.AuthorId = model.AuthorId;
                x.FilePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Files",
                    x.TempId.ToString());

                var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), x.TempId.ToString()));
                if (tempFi.Exists)
                {
                    var destFi = new FileInfo(x.FilePath);
                    if (destFi.Directory != null && !destFi.Directory.Exists)
                        destFi.Directory.Create();

                    File.Move(tempFi.FullName, x.FilePath, true);
                }
            });

            var dbModel = _mapper.Map<Post>(model);
            await _context.Posts.AddAsync(dbModel);
            await _context.SaveChangesAsync();

        }

        public async Task<List<PostModel>> GetPosts(int skip, int take)
        {
            var posts = await _context.Posts
                .Include(x => x.Owner).ThenInclude(x => x.Avatar)
                .Include(x => x.Contents).AsNoTracking().OrderByDescending(x => x.Created).Skip(skip).Take(take)
                .Select(x=>_mapper.Map<PostModel>(x))
                .ToListAsync();

            return posts;
        }

        public async Task<AttachModel> GetPostContent(Guid postContentId)
        {
            var res = await _context.Contents.FirstOrDefaultAsync(x => x.Id == postContentId);
            return _mapper.Map<AttachModel>(res);
        }









    }
}
