using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class PlayerUpdateDto
    {
        public string Id { get; set; }
        public string? Nickname { get; set; }
    }
}
