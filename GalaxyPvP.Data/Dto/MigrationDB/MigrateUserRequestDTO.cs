using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data
{
    public class MigrateUserRequestDTO
    {
        public string? PlayfabID {  get; set; }
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string WalletAddress {  get; set; }
        public string EquipData {  get; set; }
        public int Trophy {  get; set; }
        public int TotalGames { get; set; }
        public int MVP {  get; set; }
        public int Win {  get; set; }
        public int WinStreak {  get; set; }
        public int WinStreakCurrent {  get; set; }
        public short Tutorial {  get; set; }
        public short isAdmin {  get; set; }
        public short Developer {  get; set; }
        public string Verification { get; set; }
        public string[] PlayerItems {  get; set; }
    }
}
