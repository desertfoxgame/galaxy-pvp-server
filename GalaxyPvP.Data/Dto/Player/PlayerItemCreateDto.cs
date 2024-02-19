using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data.Dto.Player
{
    public class PlayerItemCreateDto 
    {
        public int DataId { get; set; }

        public string? NftType { get; set; }

        public string? NftId { get; set; }

        public short? InventoryType { get; set; } = 0;

        public int? Level { get; set; } = 1;

        public int? Exp { get; set; } = 0;
    }
}
