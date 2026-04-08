using System;
using System.ComponentModel.DataAnnotations;

namespace SIMS.Validations
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                // 1. Kiểm tra ngày sinh trong tương lai
                if (dateOfBirth > DateTime.Now)
                {
                    return new ValidationResult("Ngày sinh không hợp lệ (không thể chọn ngày trong tương lai).");
                }

                // 2. Kiểm tra độ tuổi tối thiểu
                var age = DateTime.Today.Year - dateOfBirth.Year;
                if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--; // Trừ 1 nếu chưa tới sinh nhật năm nay

                if (age < _minimumAge)
                {
                    return new ValidationResult($"Người dùng phải đủ {_minimumAge} tuổi.");
                }
            }
            return ValidationResult.Success;
        }
    }
}