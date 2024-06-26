﻿using Microsoft.AspNetCore.Authorization;
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
        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new UserVM
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Avatar = user.Avatar,
                Role = userRoles.FirstOrDefault()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["success"] = "User Updated Successfully";
                return RedirectToAction("Users");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                TempData["error"] = "Error Occurs...";
                return View(model);
            }
        }
        //[HttpDelete]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["success"] = "User deleted successfully.";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                TempData["error"] = "An error occurred while deleting the user.";
            }

            return RedirectToAction("Users");
        }
        [HttpGet]
        public async Task<IActionResult> AssignRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = _roleManager.Roles.ToList();

            var model = new RoleVM
            {
                UserId = user.Id,
                User = user,
                Roles = roles,
                UserRoles = (List<string>)await _userManager.GetRolesAsync(user)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                ModelState.AddModelError("", $"Role '{roleName}' does not exist.");
                TempData["error"] = "Error Occurs...";
                return View();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            foreach (var currentRole in userRoles)
            {
                if (currentRole == "Student")
                {
                    var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
                    if (student != null)
                    {
                        _context.Students.Remove(student);
                    }
                }
                else if (currentRole == "Professor")
                {
                    var professor = await _context.Professors.FirstOrDefaultAsync(p => p.UserId == user.Id);
                    if (professor != null)
                    {
                        _context.Professors.Remove(professor);
                    }
                }
            }
            if (roleName == "Student")
            {

                var newStudent = new Student
                {
                    UserId = user.Id,
                };
                await _context.Students.AddAsync(newStudent);
            }
            else if (roleName == "Professor")
            {
                var newProfessor = new Professor { UserId = user.Id, };

                await _context.Professors.AddAsync(newProfessor);
            }
            await _userManager.AddToRoleAsync(user, roleName);

            TempData["success"] = "Assigning Role Successfully";

            return RedirectToAction("Users");
        }

    }
}
