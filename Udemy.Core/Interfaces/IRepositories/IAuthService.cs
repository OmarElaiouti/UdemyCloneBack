using Udemy.Core.Models.AuthModel;

namespace Udemy.Core.Interfaces.IRepositories
{
    public interface IAuthService
    {
        Task<AuthModel> Register(RegisterModel model);

        Task<AuthModel> Login(LoginModel model);

        Task<string> DecodeTokenAsync(string token);
    }
}
