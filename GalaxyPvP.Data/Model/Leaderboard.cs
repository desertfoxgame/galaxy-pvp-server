using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data.Model
{
    public class Leaderboard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        [Required]
        public string PlayerId { get; set; }
        [ForeignKey("PlayerId")]
        public Player Player { get; set; }
        public string DisplayName { get; set; }
        public int Rank { get; set; }

    }
}
