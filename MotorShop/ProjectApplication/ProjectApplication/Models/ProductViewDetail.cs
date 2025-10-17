using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectApplication.Models
{
    public class ProductViewDetail
    {
        [Display(Name = "Mã sản phẩm")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hãng.")]
        [Display(Name = "Mã Hãng")]
        public int BrandId { get; set; }

        [Display(Name = "Hãng")]
        public string BrandName { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(1000, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 1.000 ₫.")]
        [Display(Name = "Giá")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm.")]
        [Display(Name = "Loại sản phẩm")]
        public string ProductType { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        [Display(Name = "Mô tả sản phẩm")]
        public string ProductDescription { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm.")]
        [Display(Name = "Số lượng trong kho")]
        public int ProductQuantity { get; set; }

        // Nếu sản phẩm là Vehicle
        [Display(Name = "Động cơ (cc)")]
        [Range(50, 2000, ErrorMessage = "Động cơ phải nằm trong khoảng 50cc - 2000cc.")]
        public int Engine { get; set; }

        [Display(Name = "Loại xe")]
        [StringLength(50, ErrorMessage = "Loại xe không được vượt quá 50 ký tự.")]
        public string VehicleType { get; set; }

        [Display(Name = "Dung tích xăng (lít)")]
        [Range(0.5, 50, ErrorMessage = "Dung tích xăng phải từ 0.5 đến 50 lít.")]
        public float FuelCapacity { get; set; }

        [Display(Name = "Màu xe")]
        public vehicleColor Color { get; set; }
    }
}
