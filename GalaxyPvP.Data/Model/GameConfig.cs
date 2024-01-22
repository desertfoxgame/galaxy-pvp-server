using System.ComponentModel.DataAnnotations;

namespace GalaxyPvP.Data
{
    public class GameConfig
    {
        [Key]
        public string Key {  get; set; }
        public string Value { get; set; }
    }
}
