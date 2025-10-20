using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class Brand
    {
        public int BrandId { get; set; } // Khóa chính của hãng
        public string BrandName { get; set; } // Tên hãng
        public string BrandDescription { get; set; } // Mô tả của hãng 

    }
}