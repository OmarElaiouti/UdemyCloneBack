using System.ComponentModel.DataAnnotations;

namespace Udemy.Core.Models.AuthModel
{
    public class RegisterModel
    {
        [Required, StringLength(250)]
        public string UserName { get; set; }

        [Required, StringLength(128)]
        public string Email { get; set; }

        [Required, StringLength(256)]
        public string Password { get; set; }





    }
}
