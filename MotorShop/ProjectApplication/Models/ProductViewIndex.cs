using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class ProductViewIndex
    {
        public int ProductId { get; set; }
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }
        [Display(Name = "Giá sản phẩm")]
        public decimal ProductPrice { get; set; }
        [Display(Name = "Còn lại")]
        public int ProductQuantity { get; set; }

        [Display(Name = "Giá gốc")]
        public decimal? OriginalPrice { get; set; } // chỉ hiển thị nếu có giảm

        public bool HasDiscount { get; set; } // có đang được khuyến mãi không

        // Thuộc tính để upload file
        public HttpPostedFileBase ImageFile { get; set; }

        // Thuộc tính để lưu đường dẫn trong DB
        public string ImagePath { get; set; }
        public string DiscountImagePath { get; set; }

    }
}