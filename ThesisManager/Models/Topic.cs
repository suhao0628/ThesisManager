using System.ComponentModel.DataAnnotations;
using ThesisManager.Models.Enum;

namespace ThesisManager.Models
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<Category>? Categories { get; set; }
        public User? Author { get; set; }
        public Student? Students { get; set; }
        public Professor? Professor { get; set; }
        public bool? IsSupervisor { get; set; }
        public List<Comment>? Comments { get; set; }
        public TopicStatus? TStatus { get; set; } = TopicStatus.Available;
        public RequestStatus? RStatus { get; set; }
        public DateTime? AppliedTime { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public bool? IsAccepted { get; set; }
        public bool? IsProposal { get; set; }
    }
}
