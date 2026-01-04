using System.ComponentModel.DataAnnotations;

namespace FruitablesFrontToBack.ViewModels.AccountVMs
{
    public class LoginVM
    {
        [Required]
        public string UsernameOrEmail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
