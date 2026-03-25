using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("permissions")]
    public class Permission : Actionable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        private int id;

        public int Id { get => id; set => id = value; }

        [Required, MaxLength(100)]
        public string Resource { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = [];
    }
}
