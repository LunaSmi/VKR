using System.Drawing;
using System.Xml.Linq;

namespace VKR.API.Models.Attach
{
    public class MetadataWithPath : MetaDataModel
    {
        public string FilePath { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public MetadataWithPath(MetaDataModel model, Func<MetaDataModel, string> pathgen, Guid authorId)
        {
            TempId = model.TempId;
            Name = model.Name;
            MimeType = model.MimeType;
            Size = model.Size;
            FilePath = pathgen(model);
            AuthorId = authorId;
        }
    }
}
