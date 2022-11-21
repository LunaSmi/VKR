using VKR.API.Models.User;

namespace VKR.API.Models.Post
{
    public class CommentModel
    {
        public Guid Id { get; set; }
        public string? Text { get; set; }
        public DateTimeOffset PublicationDate { get; set; }

        public UserAvatarForCommentModel Author { get; set; } = null!;

    }
}
