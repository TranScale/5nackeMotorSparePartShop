using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; } // Khóa chính

        [Required(ErrorMessage = "Tên chương trình giảm giá là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên giảm giá không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên giảm giá")]
        public string DiscountName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime DateStart { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày kết thúc.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày kết thúc")]
        [DateEndAfterStart("DateStart", ErrorMessage = "Ngày kết thúc phải sau ngày bắt đầu.")]
        public DateTime DateEnd { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại giảm giá.")]
        [Display(Name = "Loại giảm giá")]
        public string DiscountType { get; set; } // (Coupon, Promotion, ...)

        [Required(ErrorMessage = "Vui lòng nhập giá trị giảm.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị giảm phải lớn hơn 0.")]
        [Display(Name = "Giá trị giảm")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình thức giảm.")]
        [Display(Name = "Hình thức giảm")]
        public DiscountValueType DiscountValueType { get; set; } // (percent, directly)

        [Display(Name = "Kích hoạt")]
        public bool isActive { get; set; }
    }

    // ✅ Custom validation: đảm bảo DateEnd > DateStart
    public class DateEndAfterStartAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        public DateEndAfterStartAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var endDate = (DateTime?)value;
            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            var startDate = (DateTime?)startDateProperty?.GetValue(validationContext.ObjectInstance);

            if (startDate.HasValue && endDate.HasValue && endDate.Value < startDate.Value)
            {
                return new ValidationResult(ErrorMessage ?? "Ngày kết thúc phải sau ngày bắt đầu.");
            }

            return ValidationResult.Success;
        }
    }
}
