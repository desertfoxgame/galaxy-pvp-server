using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class AdminUpdatePlayerDTO
    {
        public GalaxyUser User { get; set; }
        public PlayerDto Player { get; set; }
    }
}
