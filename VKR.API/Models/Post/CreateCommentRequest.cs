using System.ComponentModel.DataAnnotations;

namespace VKR.API.Models.Post
{
    public class CreateCommentRequest
    {
        public Guid PostId { get; set; }
        public Guid AuthorId { get; set; }

        [MaxLength(2000)]
        public string? Text { get; set; }

    }
}
