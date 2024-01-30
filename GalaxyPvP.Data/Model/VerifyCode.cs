using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data.Model
{
    public class VerifyCode
    {
        [Key]
        public string UserId { get; set; }
        [MaxLength(20)]
        public string Code { get; set; }

    }
}
