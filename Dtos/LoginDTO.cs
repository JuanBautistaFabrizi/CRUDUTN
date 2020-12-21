using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Dtos
{
    public class LoginDTO
    {
        [Required]
        public string Password { get; set; }
        
        [Required]
        public string Nickname { get; set; }
        
        
    }
}