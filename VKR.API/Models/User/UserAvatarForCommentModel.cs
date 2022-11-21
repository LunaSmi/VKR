namespace VKR.API.Models.User
{
    public class UserAvatarForCommentModel
    {
        public Guid AuthorId { get; set; } 
        public string AuthorName { get; set; } = null!;
        public string? AvatarLink { get; set; }

    }
}
