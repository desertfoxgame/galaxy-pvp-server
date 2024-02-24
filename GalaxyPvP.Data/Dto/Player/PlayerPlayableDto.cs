using GalaxyPvP.Data.DTO;
using GalaxyPvP.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data.Dto.Player
{
    public class PlayerPlayableDto
    {
        public bool isPlayable { get; set; }
        public string banReason { get; set; }
        public long banTime { get; set; }
    }
}
