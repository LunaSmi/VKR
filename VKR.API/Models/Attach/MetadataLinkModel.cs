using System.Drawing;
using System.Xml.Linq;

namespace VKR.API.Models.Attach
{
    public class MetadataLinkModel : MetadataModel
    {
        public string FilePath { get; set; } = null!;
        public Guid AuthorId { get; set; }
    }
}
