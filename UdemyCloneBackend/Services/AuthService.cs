using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UdemyCloneBackend.Helper;
using UdemyCloneBackend.Models;
using UdemyCloneBackend.Models.AuthModel;

namespace UdemyCloneBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly JWT _jwt;

        public AuthService(UserManager<User> userManager, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
        }

        public async Task<AuthModel> Register(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)

                return new AuthModel { Message = "Email is already registered" };

            if (await _userManager.FindByNameAsync(model.UserName) is not null)

                return new AuthModel { Message = "Username is already registered" };


            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,

            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }

                return new AuthModel { Message = errors };
            }
            await _userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJWtToken(user);

            return new AuthModel
            {
                Email = user.Email,
                UserName = user.UserName,
                isAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,

            };

        }

        public async Task<AuthModel> Login (LoginModel model)
        {
            var authmodel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);
            
            var password = await _userManager.CheckPasswordAsync(user , model.Password);

            if(user == null || !password )
            {
                authmodel.Message = "Email or Password is incorrect";
                return authmodel;
            }

            var jwtSecurityToken = await CreateJWtToken(user);

            authmodel.isAuthenticated = true;
            authmodel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authmodel.Email = model.Email;  
            authmodel.ExpiresOn = jwtSecurityToken.ValidTo;

            var roleList = await _userManager.GetRolesAsync(user);
            authmodel.Roles = roleList.ToList();

            return authmodel;
        }


        private async Task<JwtSecurityToken> CreateJWtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();


            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));


            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("UserId" , user.Id),
            }
               .Union(userClaims)
               .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredintials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtsecurityToken = new JwtSecurityToken(

                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredintials
                );

            return jwtsecurityToken;
        }




    }
}
