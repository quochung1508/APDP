// File: Models/CreateAccountViewModel.cs

using System;
using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class CreateAccountViewModel
    {
        [Required]
        public string Role { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        // Add back the new fields from ApplicationUser (will not be saved to DB due to [NotMapped])
        [Display(Name = "Profile Id")]
        public string? ProfileId { get; set; }

        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}
