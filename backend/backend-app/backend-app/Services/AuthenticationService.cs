namespace smartstock_inventory_service.Services
{
    public partial class AuthenticationService
    {
        public async Task<string> GetName()
        {
            return await Task.Run(() => "luyando");
        }

        public async Task<object> Login(string username, string password)
        {
            return await Task.Run(() =>
            {
                if (username == "martha" && password == "martha123")
                {
                    return (object)new { token = "simulatedtoken123456789012345" };
                }
                return null;
            });
        }
    }
}