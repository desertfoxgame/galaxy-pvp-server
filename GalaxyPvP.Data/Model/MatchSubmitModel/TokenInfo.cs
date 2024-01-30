using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class TokenInfo
    {
        public string tokenId { get; set; }

        public string tokenAddress { get; set; }

        public int tokenType { get; set; }

        public bool isRented { get; set; }

        public bool Equals(TokenInfo obj)
        {
            if (tokenId != obj.tokenId || tokenAddress != obj.tokenAddress
                || tokenType != obj.tokenType || isRented != obj.isRented)
                return false;
            return true;
        }
    }
}
