using backend_app.Context;
using backend_app.DTOs;
using backend_app.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_app.Services
{
    public partial class AuthenticationService (ApplicationDBContext applicationDBContext, ILogger<AuthenticationService> logger)
    {

        public async Task<UserToken> Login(LoginRequest loginRequest)
        {
            var user = await applicationDBContext.Users
                .Include(u => u.Role.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Where(x => x.Username == loginRequest.Username && x.PasswordHash == loginRequest.Password)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            user.IsLoggedIn = true;
            user.LastLoginAt = DateTime.Now.ToUniversalTime();

            logger.LogInformation("model at this point {}", user);
            await applicationDBContext.SaveChangesAsync();
            
            return new UserToken()
            {
                Id = user.Id,
                Username = user.Username,
                Role = [.. user.Role.RolePermissions
                    .Select(rp => new
                    {
                        rp.Role.Name,
                        rp.Permission.Resource,
                        rp.Permission.Description
                    })]
            };
        } 

        public async Task<ActionResponseDTO> LogOut(int id)
        {
            var user = await applicationDBContext.Users
                .Where(x => x.Id == id && x.IsLoggedIn)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            user.IsLoggedIn = false;
            await applicationDBContext.SaveChangesAsync();
            
            return new() 
            { 
                Result = true,
                Message = "Logged out successfully"
            };
        }


        public async Task<bool> IsLoggedIn(int id)
        {
            var user = await applicationDBContext.Users
                .Where(x => x.Id == id && x.IsLoggedIn)
                .FirstOrDefaultAsync();

            if (user == null)
                return false;

            return true;
        }



        public async Task<User> RegisterUser(RegistrationDTO userDTO)
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