// File: Models/StudentListViewModel.cs

using System;

namespace SIMS.Models
{
    public class StudentListViewModel
    {
        public long UserId { get; set; } // Dùng để tạo link Edit/Delete
        public string? StudentNumber { get; set; } = "";
        public string? FullName { get; set; } = "";
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; } = "";
        public string? Address { get; set; } = "";
        public string? PhoneNumber { get; set; } = "";
    }
}
