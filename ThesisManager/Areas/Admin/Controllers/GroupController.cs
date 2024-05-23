using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThesisManager.Data;
using ThesisManager.Models;
using ThesisManager.ViewModels;

namespace ThesisManager.Areas.Admin.Controllers
{
    public class GroupController : Controller
    {
        private readonly AppDbContext _context;
        public GroupController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var groups = _context.Groups.Include(g => g.Students).OrderBy(g => g.GroupName).ToList();
            return View(groups);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("GroupName", "BGColor")] GroupCreateVM createGroup)
        {
            bool groupNameExists = _context.Groups.Any(g => g.GroupName == createGroup.GroupName);

            if (groupNameExists)
            {
                ModelState.AddModelError("GroupName", "A group with this name already exists.");
                return View(createGroup);
            }
            if (ModelState.IsValid)
            {
                Group group = new()
                {
                    Id = createGroup.Id,
                    GroupName = createGroup.GroupName,
                    BGColor = createGroup.BGColor,
                };
                _context.Groups.Add(group);
                _context.SaveChanges();
                TempData["success"] = "Group Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(createGroup);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var group = _context.Groups.Find(id);
            if (group == null)
                return NotFound();

            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,GroupName")] Group group)
        {
            if (id != group.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(group);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(group);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var group = _context.Groups.Find(id);
            if (group != null)
            {
                _context.Groups.Remove(group);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }


        [HttpGet]
        public IActionResult GetStudentCount(int groupId)
        {
            var group = _context.Groups
                .Include(g => g.Students) 
                .FirstOrDefault(g => g.Id == groupId);

            if (group == null)
            {
                return NotFound();
            }

            int studentCount = group.Students.Count;

            return Json(new { studentCount });
        }
        

    }
}
