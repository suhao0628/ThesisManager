using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisManager.Data;
using ThesisManager.Models;
using ThesisManager.ViewModels;

namespace ThesisManager.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signinManager;

        private readonly AppDbContext _context;
        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, SignInManager<User> signinManager, AppDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signinManager = signinManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();

            var userList = new List<UserVM>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var userVM = new UserVM
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    CreatedTime = user.CreatedTime.Date,
                    Role = userRoles.FirstOrDefault(),

                };

                userList.Add(userVM);
            }

            return View(userList);
        }
    }
}
