using backend_app.Context;
using backend_app.Models;

namespace smartstock_inventory_service.Services
{
    public partial class AuthenticationService (ApplicationDBContext applicationDBContext)
    {

        public async Task<object> Login(LoginRequest loginRequest)
        {
            //fetch user and compare login credentials
            return await Task.Run(() =>
            {
                if (loginRequest.Username == "martha" && loginRequest.Password == "martha123")
                {
                    return new { token = "simulatedtoken123456789012345" };
                }
                return null;
            });
        }

        public async Task<string> RegisterUser(UserDTO userDTO)
        {
            //create user and return updated model
            return await Task.Run(() => "Successful");
        }
    }
}