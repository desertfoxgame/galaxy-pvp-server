using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class PlayerRegisterMatchDto
    {
        public string playfabId { get; set; }
        public string sessionTicket { get; set; }
        public string matchId { get; set; }
        public string matchType { get; set; }
    }
}
