using Microsoft.AspNetCore.Identity;

namespace IdentityUsage.Models
{
    public class UserPersonalization : IdentityUser<int>
    {
        public string Cpf { get; set; }

        public string Name { get; set; }
    }
}
