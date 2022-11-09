using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKR.DAL.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTimeOffset PublicationDate { get; set; }


        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;

        public Guid PostId { get; set; }
        public virtual Post Post { get; set; } = null!;
    }
}
