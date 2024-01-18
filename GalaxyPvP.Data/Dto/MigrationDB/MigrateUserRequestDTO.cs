using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class MigrateUserRequestDTO
    {
        public string PlayfabID {  get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string WalletAddress {  get; set; }
        public int WinGames {  get; set; }
        public int TotalGames { get; set; }
        public int MVP {  get; set; }
        public int WinStreaks {  get; set; }
        public int CurrentWinStreak {  get; set; }

    }
}
