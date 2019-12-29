using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MoneySystemServer.Data;
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
        private readonly TokenValidationParameters TokenValidationParameters;
        private readonly ApplicationDbContext Context;
        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings, 
            TokenValidationParameters tokenValidationParameters, ApplicationDbContext context)
        {
            UserManager = userManager;
            JwtSettings = jwtSettings;
            TokenValidationParameters = tokenValidationParameters;
            Context = context;
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

            return await AuthentiocationResultForUser(newUser);
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

            return await AuthentiocationResultForUser(user);
        }

        private async Task<AouthenticationResoult> AuthentiocationResultForUser(IdentityUser newUser)
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
                Expires = DateTime.UtcNow.Add(JwtSettings.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = newUser.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await Context.RefreshTokens.AddAsync(refreshToken);
            await Context.SaveChangesAsync();

            return new AouthenticationResoult
            {
                Sucsses = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<AouthenticationResoult> RefreshTokenAsync(string token, string refreshToken)
        {
            var valditatedToken = GetPrincipalFromToken(token);

            if (valditatedToken == null)
            {
                return new AouthenticationResoult { Errors = new[] { "Invalid token" } };
            }

            var expiryDateUnix =
                long.Parse(valditatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateUtc > DateTime.Now)
            {
                return new AouthenticationResoult { Errors = new[] { "This token hasn't expired yet" } };
            }

            var jti = valditatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value;
            var storedRefreshToken = Context.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AouthenticationResoult { Errors = new[] { "This refresh token doesn't exists" } };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AouthenticationResoult { Errors = new[] { "This refresh token has expired" } };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AouthenticationResoult { Errors = new[] { "This refresh token has invlidated" } };
            }

            if (storedRefreshToken.Used)
            {
                return new AouthenticationResoult { Errors = new[] { "This refresh token has been used" } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AouthenticationResoult { Errors = new[] { "This refresh token does not math this JWT" } };
            }

            storedRefreshToken.Used = true;
            Context.RefreshTokens.Update(storedRefreshToken);
            await Context.SaveChangesAsync();

            var user = await UserManager.FindByIdAsync(valditatedToken.Claims.Single(x => x.Type == "id").Value);

            return await AuthentiocationResultForUser(user);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, TokenValidationParameters, out var validateToken);
                if (!IsJwtWithSecurityAlgoritm(validateToken))
                {
                    return null;
                }
                else
                {
                    return principal;
                }
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithSecurityAlgoritm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
