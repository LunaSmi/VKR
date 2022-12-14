using VKR.API.Models.Attach;
using VKR.API.Models.User;

namespace VKR.API.Models.Post
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public UserAvatarModel Author { get; set; } = null!;
        public List<AttachModelWithLink>? Contents { get; set; } = new List<AttachModelWithLink>();
        
        public long LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }

        public long CommentsCount { get; set; }
        public List<CommentModel> Comments { get; set; } = new List<CommentModel>();
    }
}
