using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ThesisManager.Models;

namespace ThesisManager.Data
{
    public class AppDbContext: IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options)
        {
        }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                    .Entity<IdentityRole>()
                .HasData(
                    new IdentityRole { Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
                    new IdentityRole { Name = "Professor", NormalizedName = "PROFESSOR" },
                    new IdentityRole { Name = "Student", NormalizedName = "STUDENT" }
                );
        }
    }
}
