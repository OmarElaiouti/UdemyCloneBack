using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Udemy.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthService> _logger;
        private readonly IUnitOfWork<FlyYallaContext> _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly JWT _jwt;
        private readonly AppSettings _appSettings;
        private readonly SmtpSettings _smtpSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IUnitOfWork<FlyYallaContext> unitOfWork,
            IEmailService emailService,
            RoleManager<IdentityRole> roleManager,
            IOptions<JWT> jwt,
            ILogger<AuthService> logger,
            IOptions<AppSettings> appSettings,
            IOptions<SmtpSettings> smtpSettings)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _logger = logger;
            _appSettings = appSettings.Value;
            _smtpSettings = smtpSettings.Value;

        }

        public async Task<RegistrationResult> RegisterAsync(RegisterDto model)
        {
            // Check if the user already exists
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                // User already exists
                return new RegistrationResult { Status = RegistrationStatus.UserAlreadyExists, Message = "User already exists." };
            }

            var passwordValidator = new PasswordValidator<ApplicationUser>();
            var validationResult = await passwordValidator.ValidateAsync(_userManager, null, model.Password);
            if (!validationResult.Succeeded)
            {
                // Password does not meet the policy requirements
                // Log the validation errors
                foreach (var error in validationResult.Errors)
                {
                    _logger.LogWarning($"Password validation error: {error.Description}");
                }
                return new RegistrationResult { Status = RegistrationStatus.PasswordValidationFailed, Message = "Password validation failed." };
            }

            // Create a new user object
            var newUser = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.DialingCode + model.MobileNumber
            };

            try
            {
                await _unitOfWork.CreateTransactionAsync(); // Start transaction

                // Attempt to create the user with the provided password
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to create role.");

                    // Rollback transaction and return false
                    await _unitOfWork.RollbackAsync();
                    return new RegistrationResult { Status = RegistrationStatus.OtherError, Message = "Failed to create user." };
                }

                // Check if the role exists
                var roleExists = await _roleManager.RoleExistsAsync(Roles.Client);
                if (!roleExists)
                {
                    // Role does not exist, create it
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(Roles.Client));
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogWarning("Failed to create role.");
                        await _unitOfWork.RollbackAsync();
                        return new RegistrationResult { Status = RegistrationStatus.OtherError, Message = "Failed to create role." };
                    }
                }

                var addToRoleResult = await _userManager.AddToRoleAsync(newUser, "Client");
                if (!addToRoleResult.Succeeded)
                {
                    _logger.LogWarning("Failed to add user to role.");
                    await _unitOfWork.RollbackAsync();
                    return new RegistrationResult { Status = RegistrationStatus.OtherError, Message = "Failed to add user to role." };
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var encodedToken = WebUtility.UrlEncode(token);

                // Construct and send email message
                if (!await SendEmailConfirmationMessageAsync(newUser.Email, newUser.FirstName, newUser.Id, encodedToken))
                {
                    // Failed to send email
                    _logger.LogWarning("Failed to send email confirmation message.");
                    await _unitOfWork.RollbackAsync();
                    return new RegistrationResult { Status = RegistrationStatus.OtherError, Message = "Failed to send email confirmation message." };
                }

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync(); // Commit transaction

                return new RegistrationResult { Status = RegistrationStatus.Success, Message = "User registered successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during register.");
                await _unitOfWork.RollbackAsync();
                return new RegistrationResult { Status = RegistrationStatus.OtherError, Message = "An error occurred during registration." };
            }
        }
        public async Task<string> LoginAsync(LoginDto model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    _logger.LogWarning("Invalid login attempt: User not found or invalid password.");
                    return null;

                }

                    
                var token = await GenerateJwtToken(user);
                return token;
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                _logger.LogError(ex, "An error occurred during login."); 
                return null;
            }
        }
        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return true; // Indicate success to prevent user enumeration
                }

                // Generate a temporary password
                var temporaryPassword = GenerateTemporaryPassword();



                #region Changing the passwor with redirect to a layout to enter the new password

                //var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                //var encodedToken = WebUtility.UrlEncode(token);


                //var callbackUrl = $"{_appSettings.ApplicationBaseUrl}/auth/reset-password?id={encodedToken}";

                //var message = new MailMessage(_smtpSettings.Username, email);
                //message.Subject = "Reset Password";
                //message.Body = $"Dear {user.FirstName},\n\n" +
                //           $"You can reset your passowrd by clicking on the following link:\n" +
                //           $"{callbackUrl}\n\n" +
                //           $"Regards,\nnFlyYalla Team";
                //message.IsBodyHtml = false;

                #endregion

                // Construct the email message with the temporary password
                var message = new MailMessage(_smtpSettings.Username,email);
                message.Subject = "Temporary Password";
                message.Body = $"Dear {user.FirstName},\n\n"
                             + $"Your temporary password is: {temporaryPassword}\n\n"
                             + $"Please log in using this temporary password and reset your password immediately.\n\n"
                             + $"Regards,\nFlyYalla Team";
                message.IsBodyHtml = false;

                // Send the email using the email service
                await _emailService.SendEmailAsync(message);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing forgot password request.");
                return false;
            }
        }
        public async Task<ChangePasswordResult> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            try
            {
                // Validate input parameters
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
                {
                    throw new ArgumentException("Invalid input parameters. Please provide valid values for userId, token, and newPassword.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    // User not found
                    return new ChangePasswordResult { Success = false, ErrorMessage = "User not found." };
                }

                // Validate the new password
                var passwordValidator = new PasswordValidator<ApplicationUser>();
                var validationResult = await passwordValidator.ValidateAsync(_userManager, null, newPassword);
                if (!validationResult.Succeeded)
                {
                    // Password does not meet the policy requirements
                    // Log the validation errors
                    var errorMessages = validationResult.Errors.Select(error => error.Description);
                    return new ChangePasswordResult { Success = false, ErrorMessage = string.Join(Environment.NewLine, errorMessages) };
                }

                var decodedToken = WebUtility.UrlDecode(token);
                // Reset the user's password using the provided token
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (!result.Succeeded)
                {
                    _logger.LogError($"Failed to reset password for user '{user.UserName}'. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    return new ChangePasswordResult { Success = false, ErrorMessage = "Password change failed." };
                }

                return new ChangePasswordResult { Success = true };
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid input parameters.");
                return new ChangePasswordResult { Success = false, ErrorMessage = "An error occurred while changing user password." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resetting user password.");
                return new ChangePasswordResult { Success = false, ErrorMessage = "An error occurred while changing user password." };
            }
        }
        public async Task<ChangePasswordResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    // User not found
                    return new ChangePasswordResult { Success = false, ErrorMessage = "User not found." };
                }

                // Validate the new password
                var passwordValidator = new PasswordValidator<ApplicationUser>();
                var validationResult = await passwordValidator.ValidateAsync(_userManager, null, newPassword);
                if (!validationResult.Succeeded)
                {
                    // Password does not meet the policy requirements
                    // Log the validation errors
                    var errorMessages = validationResult.Errors.Select(error => error.Description);
                    return new ChangePasswordResult { Success = false, ErrorMessage = string.Join(Environment.NewLine, errorMessages) };
                }

                // Verify the user's current password
                var result = await _userManager.CheckPasswordAsync(user, currentPassword);
                if (!result)
                {
                    // Current password is incorrect
                    return new ChangePasswordResult { Success = false, ErrorMessage = "Current password is incorrect." };
                }

                // Change the user's password
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if (!changePasswordResult.Succeeded)
                {
                    // Password change failed
                    return new ChangePasswordResult { Success = false, ErrorMessage = "Password change failed." };
                }

                return new ChangePasswordResult { Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing user password.");
                return new ChangePasswordResult { Success = false, ErrorMessage = "An error occurred while changing user password." };
            }
        }
        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    // User not found
                    return false;
                }

                var decodedToken = WebUtility.UrlDecode(token);

                //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                await _unitOfWork.CreateTransactionAsync();

                // Confirm the user's email using the provided token
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    // Failed to confirm email
                    _logger.LogError($"Failed to confirm email of {user.Email}");
                    await _unitOfWork.RollbackAsync();
                    return false;
                }
                user.IsEmailConfirmed = true;
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while confirming user email.");
                return false;
            }
        }

        #region private methods

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {


            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();


            foreach (var role in roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("email_confirmed", user.IsEmailConfirmed ? "true" : "false", ClaimValueTypes.String)

            }
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredintials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            
            var jwtToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredintials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);


        }
        private string GenerateTemporaryPassword()
        {
            // Generate a random temporary password (e.g., using a GUID)
            return Guid.NewGuid().ToString().Substring(0, 8); // Example: take the first 8 characters of a GUID
        }
        private async Task<bool> SendEmailConfirmationMessageAsync(string emailAddress, string firstName,string userId, string encodedToken)
        {
            try
            {
                // Construct email confirmation link
                var callbackUrl = $"{_appSettings.ApplicationBaseUrl}/auth/confirm-email?user={userId}&id={encodedToken}";

                // Construct the email message
                var message = new MailMessage(_smtpSettings.Username, emailAddress)
                {
                    
                    Subject = "Confirm your email address",
                    Body = $"Dear {firstName},\n\n" +
                           $"Thank you for registering. Please confirm your email address by clicking on the following link:\n" +
                           $"{callbackUrl}\n\n" +
                           $"Regards,\nFlytYalla Team",
                    IsBodyHtml = false
                };
               

                // Send the email using the email service
                await _emailService.SendEmailAsync(message);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending email confirmation message.");
                return false;
            }
        }

        #endregion
    }
}

