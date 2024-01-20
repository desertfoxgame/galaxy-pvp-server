using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyPvP.Data.Model
{
    public class PlayerItem:BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Player")]
        public string PlayerId { get; set; }

        [Column(TypeName = "int")]
        public int DataId { get; set; }

        [MaxLength(191)]
        public string? NftType { get; set; }

        [MaxLength(191)]
        public string? NftId { get; set; }

        [Column(TypeName = "smallint")]
        public short? InventoryType { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? Level { get; set; } = 1;

        [Column(TypeName = "int")]
        public int? Exp { get; set; } = 0;

    }
}
