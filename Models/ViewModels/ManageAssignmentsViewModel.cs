using SIMS.DatabaseContext.Entities;
using System.Collections.Generic;

namespace SIMS.Models.ViewModels
{
    public class ManageAssignmentsViewModel
    {
        public List<ClassViewModel> Classes { get; set; }

        public ManageAssignmentsViewModel()
        {
            Classes = new List<ClassViewModel>();
        }
    }

    public class ClassViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<AssignmentGroupViewModel> AssignmentGroups { get; set; }
    }

    public class AssignmentGroupViewModel
    {
        public long CourseId { get; set; }
        public string CourseName { get; set; }
        // Sử dụng tên đầy đủ để tránh xung đột namespace
        public SIMS.DatabaseContext.Entities.Teacher Teacher { get; set; }
        // Sử dụng tên đầy đủ để tránh xung đột namespace
        public List<SIMS.DatabaseContext.Entities.Student> Students { get; set; }
    }
}
