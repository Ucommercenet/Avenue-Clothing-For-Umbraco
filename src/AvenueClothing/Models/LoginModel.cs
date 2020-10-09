using System.ComponentModel.DataAnnotations;

namespace AvenueClothing.Models
{
    public class LoginViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        [Display(Name = "Stay Logged In")]
        public bool RememberMe { get; set; }
    }
}