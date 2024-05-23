using Microsoft.AspNetCore.Identity;
using ThesisManager.Data;

namespace ThesisManager.Models
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? Avatar { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
