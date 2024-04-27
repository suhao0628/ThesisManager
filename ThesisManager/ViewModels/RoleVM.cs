using Microsoft.AspNetCore.Identity;
using ThesisManager.Models;

namespace ThesisManager.ViewModels
{
    public class RoleVM
    {
        public string UserId { get; set; }
        public User User { get; set; } // user to whom the role will be assigned

        public List<IdentityRole> Roles { get; set; } // list of available roles

        public List<string> UserRoles { get; set; } // roles currently assigned to the user
    }

}
