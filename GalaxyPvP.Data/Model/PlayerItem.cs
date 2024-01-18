using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyPvP.Data.Model
{
    public class PlayerItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Player")]
        public int PlayerId { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int DataId { get; set; }

        [MaxLength(191)]
        public string? NftType { get; set; }

        [MaxLength(191)]
        public string? NftId { get; set; }

        [Column(TypeName = "smallint")]
        public short InventoryType { get; set; }

        [Column(TypeName = "int")]
        public int Level { get; set; }

        [Column(TypeName = "int")]
        public int Exp { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
