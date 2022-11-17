using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKR.DAL.Entities
{
    public class PostContent : Attach
    {
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; } = null!;

    }
}
