using System.ComponentModel.DataAnnotations;

namespace ThesisManager.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public User Author { get; set; }
        public DateTime CreatedTime { get; set; }
        public Topic Topic { get; set; }
    }
}
