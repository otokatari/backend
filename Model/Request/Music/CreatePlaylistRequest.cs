using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtokatariBackend.Model.Request.Music
{
    public class CreatePlaylistRequest
    {
        public bool Favourite { get; set; }
        public string Name { get; set; }

    }
}
