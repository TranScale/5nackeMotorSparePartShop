using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public String TypeProduct { get; set; }
        [Display(Name = "Thương hiệu")]
        public int BrandId { get; set; }
        [Display(Name = "Giá")]
        public long price { get; set; }
        [Display(Name = "Số lượng")]
        public int number { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public virtual SparePart SparePart { get; set; }
    }
}