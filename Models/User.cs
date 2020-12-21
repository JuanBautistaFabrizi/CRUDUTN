using Microsoft.AspNetCore.Identity;

namespace Infraestructura.Models
{
    public class User: IdentityUser
    {
        public string Nickname { get; set; }
    }
}