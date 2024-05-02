
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RegistrationResult> RegisterAsync(RegisterDto model); // Register a new user
        Task<string> LoginAsync(LoginDto model);    // Authenticate and generate JWT token
        Task<ChangePasswordResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> ForgotPasswordAsync(string email);     // Initiate password reset process
        Task<bool> ConfirmEmailAsync(string userId, string token);

        Task<ChangePasswordResult> ResetPasswordAsync(string userId, string token, string newPassword); // Complete password reset

    }
}
