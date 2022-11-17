using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKR.DAL.Entities
{
    public class Avatar : Attach
    {
        public Guid OwnerId { get; set; }
        public virtual User Owner { get; set; } = null!;
    }
}
