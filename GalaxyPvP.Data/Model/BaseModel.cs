using System.ComponentModel.DataAnnotations.Schema;

namespace GalaxyPvP.Data
{
    public abstract class BaseModel
    {
        private DateTime _createdAt;
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt
        {
            get => _createdAt.ToLocalTime();
            set => _createdAt = value.ToLocalTime();
        }
        private DateTime _updatedAt;
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt
        {
            get => _updatedAt.ToLocalTime();
            set => _updatedAt = value.ToLocalTime();
        }
        public bool IsDeleted { get; set; } = false;
    }
}
