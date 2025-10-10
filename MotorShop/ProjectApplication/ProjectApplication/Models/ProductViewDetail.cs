using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class ProductViewDetail
    {
        [Display(Name = "Mã sản phẩm")]
        public int ProductId { get; set; }
        [Display(Name = "Mã Hãng")]
        public int BrandId { get; set; }
        [Display(Name = "Hãng")]
        public string BrandName { get; set; }

        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }
        [Display(Name = "Giá")]
        public decimal ProductPrice { get; set; }
        [Display(Name = "Loại sản phẩm")]
        public string ProductType { get; set; }
        [Display(Name = "Mô tả sản phẩm")]
        public string ProductDescription { get; set; }
        [Display(Name = "Còn lại")]
        public int ProductQuantity { get; set; }


        //Nếu sản phẩm là Vehicle
        [Display(Name = "Động cơ")]
        public int Engine { get; set; } //Động cơ xe (vd: 110cc, 125cc,150cc,...)
        [Display(Name = "Loại xe")]
        public string VehicleType { get; set; } //Loại xe (vd: Xe số, xe tay ga, Xe côn)
        [Display(Name = "Dung tích xăng")]
        public float FuelCapacity { get; set; } //Dung tích xăng (vd: 4.5l,5.5l,...)
        [Display(Name = "Màu")]
        public vehicleColor Color { get; set; } //Màu của xe

        //public virtual ICollection<SparePart> spareParts { get; set; } 


        //Nếu sản phẩm là SparePart...

        //public virtual ICollection<Vehicle> Vehicle { get; set; }

    }
}