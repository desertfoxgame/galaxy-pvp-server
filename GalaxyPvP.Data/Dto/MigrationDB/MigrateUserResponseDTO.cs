using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data.Dto.MigrationDB
{
    public class MigrateUserResponseDTO
    {
        public string PlayfabID { get; set; }
        public string VerifyCode { get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string WalletAddress { get; set; }
        public int WinGames { get; set; }
        public int TotalGames { get; set; }
        public int MVP { get; set; }
        public int WinStreaks { get; set; }
        public int CurrentWinStreak { get; set; }
        public string[] PlayerItems { get; set; }
        public List<string> PlayerItemsCantCreate { get; set; }
    }
}
