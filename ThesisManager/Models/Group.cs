using System.ComponentModel.DataAnnotations;

namespace ThesisManager.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        public string GroupName { get; set; }
        public List<Student>? Students { get; set; }
        public string? BGColor { get; set; } = "bg-primary";
    }
}
