using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Udemy.Core.Interfaces;
using Udemy.Core.Interfaces.IRepositories;
using Udemy.Core.Models;
using Udemy.Core.Models.AuthModel;
using Udemy.Core.Models.UdemyContext;
using Udemy.EF.UnitOfWork;
using UdemyCloneBackend.Helper;

namespace Udemy.EF.Repositories
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JWT _jwt;
        private readonly IUnitOfWork<UdemyContext> _unitOfWork;
        public AuthService(UserManager<User> userManager, IOptions<JWT> jwt, SignInManager<User> signInManager, IUnitOfWork<UdemyContext> unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt.Value;
            _unitOfWork = unitOfWork;
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

            try
            {
                _unitOfWork.CreateTransaction(); // Begin transaction for data consistency

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(",", result.Errors.Select(e => e.Description))); // Combine errors for better message
                }

                await _userManager.AddToRoleAsync(user, "Student");

                var jwtSecurityToken = await CreateJWtToken(user);

                _unitOfWork.Commit(); // Commit transaction if successful

                return new AuthModel
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    isAuthenticated = true,
                    Roles = new List<string> { "Student" },
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    ExpiresOn = jwtSecurityToken.ValidTo,
                };
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback(); // Rollback on errors
                return new AuthModel { Message = "Registration failed" }; // Generic message for user-facing response
            }
        }



        public async Task<AuthModel> Login(LoginModel model)
        {
            var authModel = new AuthModel();

            try
            {

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    // Avoid revealing if username or password is incorrect for security reasons.
                    // A generic message like "Invalid login credentials" is recommended.
                    authModel.Message = "Invalid login credentials";
                    return authModel;
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    // Same as above, avoid revealing specific details.
                    authModel.Message = "Invalid login credentials";
                    return authModel;
                }

                var jwtSecurityToken = await CreateJWtToken(user);

                authModel.isAuthenticated = true;
                authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authModel.Email = model.Email;
                authModel.ExpiresOn = jwtSecurityToken.ValidTo;

                var roleList = await _userManager.GetRolesAsync(user);
                authModel.Roles = roleList.ToList();

                return authModel;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Login error: {ex.Message}");
                authModel.Message = "Login failed"; // Generic message for user
                return authModel;
            }
        }


        private async Task<JwtSecurityToken> CreateJWtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();


            foreach (var role in roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role));


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


        public async Task<string> DecodeTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                ClaimsPrincipal claimsPrincipal = await Task.Run(() => tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken));

                // Extract the user ID claim from the validated token
                Claim userIdClaim = claimsPrincipal.FindFirst("UserId");

                if (userIdClaim != null)
                {
                    return userIdClaim.Value;
                }
                else
                {
                    throw new InvalidOperationException("User ID claim not found in token.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Token validation failed.", ex);
            }
        }

    }
}
