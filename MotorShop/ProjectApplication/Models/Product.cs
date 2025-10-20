using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class Product
    {
        public string ImagePath { get; set; }
        public int ProductId { get; set; } // Khóa chính của sản phẩm
        public int BrandId { get; set; } //Khóa chính của hãng 
        public string ProductName { get; set; } //Tên sản phẩm
        public string ProductType { get; set; } //Loại sản phẩm (Vehicle,SparePart)
        public decimal Price { get; set; } // Giá tiền của sản phẩm
        public int Quantity { get; set; } //Số lượng còn lại trong kho của sản phẩm
        public string ProductDescription { get; set; } //Mô tả của sản phẩm 

        public virtual Brand brand { get; set; }
        //public Vehicle vehicle { get; set; }
        //public SparePart sparepart { get; set; }
    }
}