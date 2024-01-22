using GalaxyPvP.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data.Dto.Player
{
    public class ListPlayerItemDto
    {
        public string PlayerId { get; set; }
        public List<PlayerItem>? PlayerItems { get; set; }
    }
}
