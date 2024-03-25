using System.ComponentModel.DataAnnotations;

namespace Udemy.Core.Models.AuthModel
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
