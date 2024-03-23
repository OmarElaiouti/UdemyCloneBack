using UdemyCloneBackend.Models.AuthModel;

namespace UdemyCloneBackend.Services
{
    public interface IAuthService
    {
        Task<AuthModel> Register (RegisterModel model); 


        Task<AuthModel> Login (LoginModel model );
    }
}
