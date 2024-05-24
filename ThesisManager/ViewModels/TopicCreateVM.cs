using ThesisManager.Models.Enum;
using ThesisManager.Models;

namespace ThesisManager.ViewModels
{
    public class TopicCreateVM
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public User? Author { get; set; }
        public Category? Category { get; set; }
        public List<Student>? Students { get; set; }
        public Professor? Professor { get; set; }

        public int ProfessorId { get; set; }
        public bool IsSupervisor { get; set; } = false;
        public List<Comment>? Comments { get; set; }
        public List<string>? Categories { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public TopicStatus? TStatus { get; set; } = TopicStatus.Available;
    }
}
