namespace ThesisManager.ViewModels
{
    public class UserVM
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Email { get; set; }

        public string? Avatar { get; set; }
        public DateTime CreatedTime { get; set; }

        public string Role { get; set; }

    }
}
