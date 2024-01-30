using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class MatchMakingTicketDTO
    {
        public string ClientVersion { get; set; }
        public string Region { get; set; }
        public int ModeId { get; set; }
        public int Trophy {  get; set; }
    }
}
