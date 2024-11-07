using System.ComponentModel.DataAnnotations;

namespace IdentityUsage.Models.Request
{
    public class User
    {
        public string Cpf { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public string Nome { get; set; }
    }
}
