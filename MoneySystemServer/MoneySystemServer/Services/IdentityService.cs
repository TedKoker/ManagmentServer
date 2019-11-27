using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MoneySystemServer.Models;
using MoneySystemServer.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MoneySystemServer.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> UserManager;
        private readonly JwtSettings JwtSettings;
        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings)
        {
            UserManager = userManager;
            JwtSettings = jwtSettings;
        }
        public async Task<AouthenticationResoult> RegisterAsync(string email, string password)
        {
            var existed = await UserManager.FindByEmailAsync(email);
            if (existed != null)
            {
                return new AouthenticationResoult
                {
                    Errors = new[] { "User with this email is already exist" }
                };
            }

            var newUser = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var createdUser = await UserManager.CreateAsync(newUser, password);

            if (!createdUser.Succeeded)
            {
                return new AouthenticationResoult
                {
                    Errors = createdUser.Errors.Select(x => x.Description)
                };
            }

            return AuthentiocationResultForUser(newUser);
        }

        public async Task<AouthenticationResoult> LoginAsync(string email, string paswoord)
        {
            var user = await UserManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AouthenticationResoult
                {
                    Errors = new[] { "User does not exsist" }
                };
            }

            var userHasValidPassword = await UserManager.CheckPasswordAsync(user, paswoord);

            if (!userHasValidPassword)
            {
                return new AouthenticationResoult
                {
                    Errors = new[] { "User/Password combination is incorrect" }
                };
            }

            return AuthentiocationResultForUser(user);
        }

        private AouthenticationResoult AuthentiocationResultForUser(IdentityUser newUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, newUser.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, newUser.Email),
                    new Claim("id", newUser.Id)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AouthenticationResoult
            {
                Sucsses = true,
                Token = tokenHandler.WriteToken(token)
            };
        }
    }
}
