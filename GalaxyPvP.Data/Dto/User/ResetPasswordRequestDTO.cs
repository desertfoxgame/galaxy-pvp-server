using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data.Dto.User
{
    public class ResetPasswordRequestDTO
    {
        public string VerifyCode { get; set; }
        public string NewPassword { get; set; }
    }
}
