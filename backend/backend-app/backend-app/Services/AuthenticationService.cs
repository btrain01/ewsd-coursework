using AutoMapper;
using backend_app.Context;
using backend_app.DTOs;
using backend_app.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_app.Services
{
    public partial class AuthenticationService (ApplicationDBContext applicationDBContext, IMapper mapper, AuthenticationUserContext authenticationUserContext)
    {

        public async Task<UserToken> Login(LoginRequest loginRequest)
        {
            var user = await applicationDBContext.Users
                .Include(u => u.Role.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Where(x => x.Username == loginRequest.Username)
                .FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
                return null;

            user.IsLoggedIn = true;
            user.LastLoginAt = DateTime.Now.ToUniversalTime();

            await applicationDBContext.SaveChangesAsync();
            
            var userToken = new UserToken()
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

            return userToken;
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



        public async Task<UserToken> RegisterUser(RegistrationDTO userDTO)
        {
            var user = new User()
            {
                Username = userDTO.Username,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDTO.Password, workFactor: 12),
                Email = userDTO.Email,
                RoleId = userDTO.RoleId
            };

            applicationDBContext.Users.Add(user);

            await applicationDBContext.SaveChangesAsync();

            return mapper.Map<UserToken>(user);
        }
    }
}