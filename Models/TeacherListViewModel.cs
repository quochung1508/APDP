using SIMS.DatabaseContext.Entities;

namespace SIMS.Models
{
    public class TeacherListViewModel
    {
        // From ApplicationUser
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        // From Teacher Profile
        public string TeacherNumber { get; set; }
        public string FullName { get; set; }

        // This property will help the view decide what to display.
        public bool HasProfile { get; set; }
    }
}
