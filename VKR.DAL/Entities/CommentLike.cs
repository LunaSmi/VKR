using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKR.DAL.Entities
{
    public class CommentLike
    {
        public Guid Id { get; set; }

        public Guid CommentId { get; set; }
        public virtual Comment Comment { get; set; } = null!;

        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;

    }
}
