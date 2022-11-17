namespace VKR.API.Models.Attach
{
    public class AttachModelWithLink
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string? ContentLink { get; set; } = null!;

    }
}
