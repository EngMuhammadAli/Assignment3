using System.ComponentModel.DataAnnotations;

namespace Assignment3.Model
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
