using backend_app.Context;
using backend_app.DTOs;
using backend_app.Models;
using Microsoft.EntityFrameworkCore;

namespace smartstock_inventory_service.Services
{
    public partial class AuthenticationService (ApplicationDBContext applicationDBContext)
    {

        public async Task<UserToken> Login(LoginRequest loginRequest)
        {
            var user = await applicationDBContext.Users
                .Where(x => x.Username == loginRequest.Username && x.Password == loginRequest.Password)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            return new UserToken()
            {
                Username = user.Username,
                Role = [.. user.UserRoles.Select(x => x.Role.Name)]
            };
        }

        public async Task<User> RegisterUser(UserDTO userDTO)
        {
            var user = new User()
            {
                Username = userDTO.Username,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Password = userDTO.Password,
                Email = userDTO.Email,
                IsActive = true
            };

            applicationDBContext.Users.Add(user);

            await applicationDBContext.SaveChangesAsync();

            return user;
        }
    }
}