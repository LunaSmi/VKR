using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKR.DAL.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = "empty";
        public DateTimeOffset Created { get; set; }


        public virtual User Owner { get; set; } = null!;

        //public virtual ICollection<string>? PhotoUrls { get; set; }
        public virtual ICollection<Photo>? Photos { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

    }
}
