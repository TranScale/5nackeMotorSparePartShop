using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public int BrandId { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string Price { get; set; }
        public int Quantity { get; set; }
        public string ProductDescription { get; set; }

        public Vehicle vehicle { get; set; }
        public SparePart sparepart { get; set; }
    }
}