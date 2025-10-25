using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectApplication.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; } // Khóa chính

        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [StringLength(150, ErrorMessage = "Tên sản phẩm không được vượt quá 150 ký tự.")]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hãng sản xuất.")]
        [Display(Name = "Hãng sản xuất")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm.")]
        [StringLength(50)]
        [Display(Name = "Loại sản phẩm")]
        public string ProductType { get; set; } // Vehicle hoặc SparePart

        [Required(ErrorMessage = "Giá sản phẩm không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        [Display(Name = "Giá")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không hợp lệ.")]
        [Display(Name = "Số lượng tồn kho")]
        public int Quantity { get; set; }

        [Display(Name = "Mô tả sản phẩm")]
        public string ProductDescription { get; set; }

        [StringLength(255)]
        [Display(Name = "Đường dẫn hình ảnh")]
        public string ImagePath { get; set; }

        // 🔗 Quan hệ
        public virtual Brand brand { get; set; }
    }
}
