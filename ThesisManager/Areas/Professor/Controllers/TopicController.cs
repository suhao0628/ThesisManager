using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThesisManager.Models.Enum;
using ThesisManager.Models;
using ThesisManager.Data;
using Microsoft.EntityFrameworkCore;
using ThesisManager.ViewModels;

namespace ThesisManager.Areas.Professor.Controllers
{
    [Authorize(Roles = "Professor")]
    [Area("Professor")]
    public class TopicController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public TopicController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #region Display All Topics
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = GetAllCategories();

            var viewModel = new TopicListVM
            {
                Categories = categories,
            };
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var topics = _context.Topics
                .Include(t => t.Author) 
                .Include(t => t.Categories)
                .Where(t => t.Author.Id == user.Id)
                .ToList(); 
            viewModel.Topics = topics;
            return View(viewModel);
        }
        #endregion

        #region Create Topic


        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();

            ViewData["Categories"] = categories;

            var topicCreateVM = new TopicCreateVM();

            return View(topicCreateVM);
        }
        [HttpPost]
        public IActionResult Create(TopicCreateVM request)
        {
            var currentUser = GetCurrentUser();
            var categories = new List<Category>();
            var existingCategories = _context.Categories.ToList();

            var existingCategoryNames = new HashSet<string>(existingCategories.Select(c => c.Name));
            foreach (var categoryName in request.Categories)
            {
                Category category;
                if (categoryName != null)
                {
                    if (existingCategoryNames.Contains(categoryName))
                    {
                        category = existingCategories.FirstOrDefault(c => c.Name == categoryName);
                    }
                    else
                    {

                        category = new Category
                        {
                            Name = categoryName
                        };
                        _context.Categories.Add(category);
                        existingCategories.Add(category);

                    }

                    categories.Add(category);
                }

            }

            _context.SaveChanges();

            var topic = new Topic
            {
                Title = request.Title,
                Description = request.Description,
                CreatedTime = DateTime.Now,
                Author = currentUser,
                Categories = categories,
                TStatus = TopicStatus.Available,
            };

            _context.Topics.Add(topic);
            _context.SaveChanges();
            TempData["success"] = "Topic Created Successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit Topic


        [HttpGet]
        public IActionResult Update(int id)
        {
            var topic = _context.Topics
                .Include(t => t.Categories)
                .FirstOrDefault(t => t.Id == id);

            if (topic == null)
            {
                return NotFound();
            }

            var categories = _context.Categories.ToList();

            ViewData["Categories"] = categories;

            var topicUpdateVM = new TopicCreateVM
            {
                Id = topic.Id,
                Title = topic.Title,
                Description = topic.Description,
                CreatedTime = topic.CreatedTime,
                Author = topic.Author,
                Categories = topic.Categories.Select(c => c.Name).ToList(),
                TStatus = topic.TStatus
            };

            return View(topicUpdateVM);
        }

        [HttpPost]
        public IActionResult Update(int id, TopicCreateVM request)
        {
            var topic = _context.Topics
                .Include(t => t.Categories)
                .FirstOrDefault(t => t.Id == id);

            if (topic == null)
            {
                return NotFound();
            }

            var currentUser = GetCurrentUser();

            // Update properties of the topic entity
            topic.Title = request.Title;
            topic.Description = request.Description;

            // Update categories
            var existingCategories = _context.Categories.ToList();
            var existingCategoryNames = new HashSet<string>(existingCategories.Select(c => c.Name));
            var categories = new List<Category>();

            foreach (var categoryName in request.Categories)
            {
                Category category;
                if (categoryName != null)
                {
                    if (existingCategoryNames.Contains(categoryName))
                    {
                        category = existingCategories.FirstOrDefault(c => c.Name == categoryName);
                    }
                    else
                    {
                        category = new Category
                        {
                            Name = categoryName
                        };
                        _context.Categories.Add(category);
                        existingCategories.Add(category);

                    }

                    categories.Add(category);
                }
            }

            topic.Categories = categories;

            _context.SaveChanges();
            TempData["success"] = "Topic Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete Topic
        public IActionResult Delete(int topicId)
        {
            var topic = _context.Topics.FirstOrDefault(t => t.Id == topicId);

            if (topic == null)
            {
                return NotFound();
            }

            _context.Topics.Remove(topic);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Topic Details 
        [HttpGet]
        public IActionResult Details(int id)
        {
            var topic = _context.Topics
                .Include(t => t.Author)
                .Include(t => t.Categories)
                .FirstOrDefault(t => t.Id == id);

            if (topic == null)
            {
                return NotFound();
            }

            return View(topic);
        }
        #endregion

        #region Filter Topic
        [HttpGet]
        public IActionResult FilterTopics(string category, string professorName, TopicStatus? status)
        {

            var topics = _context.Topics
                .Include(t => t.Author)
                .Include(t => t.Categories)
                .AsQueryable(); 

            if (!string.IsNullOrEmpty(category))
            {
                topics = topics.Where(t => t.Categories.Any(c => c.Name == category));
            }

            if (!string.IsNullOrEmpty(professorName))
            {
                topics = topics.Where(t => t.Author.FirstName + " " + t.Author.LastName == professorName);
            }

            if (status != null)
            {
                topics = topics.Where(t => t.TStatus == status);
            }

            var filteredTopics = topics.ToList();
            var viewModel = new TopicListVM
            {
                Topics = filteredTopics,
                Categories = GetAllCategories(),
                Professors = GetAllProfessors()
            };

            return View("Index", viewModel);
        }

        #endregion

        #region utility
        private User GetCurrentUser()
        {
            return _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
        }
        private List<string> GetAllCategories()
        {
            return _context.Categories.Select(c => c.Name).ToList();
        }
        private List<string> GetAllProfessors()
        {
            var professorNames = _context.Professors.Select(p => $"{p.User.FirstName} {p.User.LastName}").ToList();

            return professorNames;

        }
        #endregion
    }

}
