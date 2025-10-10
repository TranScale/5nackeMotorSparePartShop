using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class Coupon
    {
        public int CouponId { get; set; }
        public string Code { get; set; } // Ví dụ: "GIAMGIA30K"
        public decimal Value { get; set; } // Số tiền giảm (Ví dụ: 30000)
        public bool IsActive { get; set; } // Trạng thái
    }
}