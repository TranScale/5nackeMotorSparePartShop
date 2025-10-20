using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class DiscountViewIndex
    {
        public int Id { get; set; } //Khóa chính: DiscountId
        [Display(Name = "Tên")]
        public string Name { get; set; } //Tên: DiscountName
        [Display(Name = "Loại khuyến mãi")]
        public string DiscountType { get; set; } // Loại giảm giá: DiscountType
        [Display(Name = "Bắt đầu")]
        public DateTime DateStart { get; set; } //Ngày bắt đầu: DateStart
        [Display(Name = "Kết thúc")]
        public DateTime DateEnd { get; set; } //Ngày kết thúc: DateEnd
        public DiscountValueType DiscountValueType { get; set; } // Loại Discount
        [Display(Name = "Giá trị")]
        public decimal DiscountValue { get; set; } //Giá trị discount: DiscountValue 
        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } // Trạng thái discount: isActive
    }
}