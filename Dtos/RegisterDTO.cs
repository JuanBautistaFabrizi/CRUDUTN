using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Dtos
{
    public class RegisterDTO
    {
        [Required]
        public string Email { get; set; }
        
        [Required]
        [StringLength(12)]
        public string Nickname { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 6)]
        public string Password { get; set; }
    }
}