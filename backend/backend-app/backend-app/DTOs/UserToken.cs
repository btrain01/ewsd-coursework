using backend_app.Models;

namespace backend_app.DTOs
{
    public class UserToken
    {
        public string Username { get; set; }    
        public List<Permission> Role { get; set; }    
    }
}
