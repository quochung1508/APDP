using System.Collections.Generic;

namespace SIMS.Models
{
    public class AssignmentUpdateViewModel
    {
        public long ClassId { get; set; }
        public long CourseId { get; set; }
        public long TeacherId { get; set; }
        public List<long> StudentIdsToAdd { get; set; }
        public List<long> StudentIdsToRemove { get; set; }

        public AssignmentUpdateViewModel()
        {
            StudentIdsToAdd = new List<long>();
            StudentIdsToRemove = new List<long>();
        }
    }
}
