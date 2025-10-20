using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class Discount
    {
        public int DiscountId { get; set; } //Khóa chính Discount 
        //[Required]
        public string DiscountName { get; set; }  // Tên giảm giá
        //[Required]
        public DateTime DateStart { get; set; } // Ngày bắt đầu 
        //[Required]
        public DateTime DateEnd { get; set; } // Ngày kết thúc 
        //[Required]
        public string DiscountType { get; set; } //Loại giảm giá (Coupon, Promotion)
        //[Required]
        public decimal DiscountValue { get; set; } // Giá trị giảm của discount (đ, %)
        //[Required]
        public DiscountValueType DiscountValueType { get; set; } // Loại giảm (percent, directly)
        public bool isActive { get; set; } // Trạng thái của discount 
        
    }
}