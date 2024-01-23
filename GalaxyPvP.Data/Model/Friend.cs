using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalaxyPvP.Data.Model
{
    public class Friend : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Player1Id {  get; set; }
        public Player Player1 {  get; set; }
        [Required]
        public string Player2Id { get; set; }
        public Player Player2 { get; set; }
        [Required]
        public short state { get; set; } = 0;

    }
}
