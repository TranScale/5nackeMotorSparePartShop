using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class Promotion : Discount
    {
        //[Required]
        public string Condition { get; set; } //Điều kiện sản phẩm 
        //[Required]
        public string PromotionDescription { get; set; } // Mô tả của Promotion

    }
}