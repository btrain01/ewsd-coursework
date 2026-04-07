namespace backend_app.Services
{
    public class AuthenticationUserContext
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public IReadOnlyList<object> Roles { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
