using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class PlayerItemDto
    {
        public int Id { get; set; }

        public string PlayerId { get; set; }

        public int DataId { get; set; }

        public string? NftType { get; set; }

        public string? NftId { get; set; }

        public short? InventoryType { get; set; } = 0;

        public int? Level { get; set; } = 1;

        public int? Exp { get; set; } = 0;
    }
}
