using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class Coupon : Discount 
    {
        //[Required]
        public string CouponCode { get; set; } // Mã của coupon 
    }
}