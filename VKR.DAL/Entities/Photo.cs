using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKR.DAL.Entities
{
    public class Photo : Attach
    {
        public virtual Post Post { get; set; } = null!;

    }
}
