using System.ComponentModel.DataAnnotations;

namespace AvenueClothing.Models
{
    public class LoginViewModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}