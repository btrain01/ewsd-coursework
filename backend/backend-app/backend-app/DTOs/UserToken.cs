namespace backend_app.DTOs
{
    public class UserToken
    {
        public int Id { get; set; }    
        public string Username { get; set; }    
        public IEnumerable<object> Role { get; set; }    
    }
}
