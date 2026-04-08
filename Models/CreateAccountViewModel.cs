using System;
using System.ComponentModel.DataAnnotations;
using SIMS.Validations;

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

        [Display(Name = "Profile Id")]
        public string? ProfileId { get; set; }

        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        [MinimumAge(18)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng số 0 và có đúng 10 chữ số.")]
        public string PhoneNumber { get; set; }
    }
}