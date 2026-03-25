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
                .Include(u => u.Role.RolePermissions)
                .Where(x => x.Username == loginRequest.Username && x.PasswordHash == loginRequest.Password)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            user.IsLoggedIn = true;
            await applicationDBContext.SaveChangesAsync();
            return new UserToken()
            {
                Username = user.Username,
                Role = [.. user.Role.RolePermissions.Select(rp => rp.Permission)]
            };
        }

        public async Task<object> LogOut(int id)
        {
            var user = await applicationDBContext.Users
                .Where(x => x.Id == id && x.IsLoggedIn)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            user.IsLoggedIn = false;
            await applicationDBContext.SaveChangesAsync();
            return new { Result = "Success"};
        }

        public async Task<User> RegisterUser(UserDTO userDTO)
        {
            var user = new User()
            {
                Username = userDTO.Username,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                PasswordHash = userDTO.Password,
                Email = userDTO.Email,
                RoleId = userDTO.RoleId
            };

            applicationDBContext.Users.Add(user);

            await applicationDBContext.SaveChangesAsync();

            return user;
        }
    }
}