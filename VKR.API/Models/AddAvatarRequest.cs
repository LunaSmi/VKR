namespace VKR.API.Models
{
    public class AddAvatarRequest
    {
        public MetaDataModel Avatar { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
