using System.ComponentModel.DataAnnotations;

namespace IdentityUsage.Models.Request
{
    public class Login
    {
        public string Email { get; set; }

        public string Senha { get; set; }
    }
}
