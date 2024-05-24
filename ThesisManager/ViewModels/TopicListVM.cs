using ThesisManager.Models;

namespace ThesisManager.ViewModels
{
    public class TopicListVM
    {
        public string? NewComment { get; set; }
        public List<Topic>? Topics { get; set; }

        public List<string>? Categories { get; set; }

        public List<string>? Professors { get; set; }

        public bool IsOpened { get; set; } = false;
        public bool IsSelected { get; set; } = false;
    }
}
