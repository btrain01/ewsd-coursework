using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend_app.Models
{
    [Table("users")]
    public class User : Actionable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Username {get; set;} = string.Empty;
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public string Email {get; set;} = string.Empty;
        
        public int RoleId { get; set; }
        
        public string PasswordHash { get; set; }
        
        public bool IsLoggedIn { get; set; }
        
        public DateTime? LastLoginAt { get; set; }

        [JsonIgnore]
        public Role Role { get; set; }

        public override string ToString()
        {
            return $"User (Id={Id}, Username={Username}, FirstName={FirstName}, LastName={LastName}, Email={Email}, RoleId={RoleId}, Password={PasswordHash}, IsLoggedIn={IsLoggedIn}, LastLoginAt={LastLoginAt}, CreatedAt={CreatedAt}, UpdatedAt={UpdatedAt})";
        }
    }
}