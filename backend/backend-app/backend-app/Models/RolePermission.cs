using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("role_permissions")]
    public class RolePermission : Actionable
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public int GrantedBy { get; set; }

        public Permission Permission { get; set; }
        public Role Role { get; set; }
    }
}
