namespace backend_app.Services
{
    public class AuthenticationUserContext
    {
        public int UserId { get; private set; }
        public string Username { get; private set; }
        public IReadOnlyList<object> Roles { get; private set; }
        public bool IsAuthenticated { get; private set; }

        public void Initialize(int userId, string username, IEnumerable<object> roles)
        {
            if (IsAuthenticated)
                throw new InvalidOperationException("UserContext has already been initialized.");

            UserId = userId;
            Username = username;
            Roles = roles?.ToList().AsReadOnly() ?? new List<object>().AsReadOnly();
            IsAuthenticated = true;
        }
    }
}
