using backend_app.Models;

namespace backend_app.DTOs
{
    public class UserToken
    {
        public int Id { get; set; }    
        public string Username { get; set; }    
        public List<object> Role { get; set; }    
    }
}
