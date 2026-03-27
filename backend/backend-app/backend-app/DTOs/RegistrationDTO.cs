using System.ComponentModel.DataAnnotations;

namespace backend_app.DTOs
{
    public class RegistrationDTO
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
