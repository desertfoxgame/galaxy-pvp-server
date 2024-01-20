using System.ComponentModel.DataAnnotations;

namespace GalaxyPvP.Data.Model
{
    public class GameConfig:BaseModel
    {
        [Key]
        public string Key {  get; set; }
        public string Value { get; set; }
    }
}
