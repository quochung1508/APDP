
// File: Models/EditUserViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class EditUserViewModel
    {
        // User's Core Info
        public long Id { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; }

        // Profile Info (Student/Teacher) - marked as NotMapped in ApplicationUser
        [Display(Name = "Student/Teacher ID")]
        public string? ProfileId { get; set; } // For Student Number or Teacher ID

        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        // Change Password Section
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string? NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
