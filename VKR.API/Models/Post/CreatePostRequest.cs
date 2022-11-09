using VKR.API.Models.Attach;

namespace VKR.API.Models.Post
{
    public class CreatePostRequest
    {
        public string? Description { get; set; }
        public List<MetaDataModel> Contents { get; set; } = new List<MetaDataModel>();
        
        //public CreatePostRequest(string description)
        //{
        //    Description = description;
        //}
    }
}
