using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class ProductViewIndex
    {
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }
        [Display(Name = "Giá sản phẩm")]
        public decimal ProductPrice { get; set; }
        [Display(Name = "Còn lại")]
        public int ProductQuantity { get; set; }

        //Xuất 1 sản phẩm
        public static ProductViewIndex GetIndexView (Product product)
        {
            return new ProductViewIndex
            {
                ProductName = product.ProductName,
                ProductPrice = product.Price,
                ProductQuantity = product.Quantity
            };
        }


        //Xuất danh sách các sản phẩm
        public static List<ProductViewIndex> GetListIndex(List<Product> product)
        {
            var list = new List<ProductViewIndex>();
            foreach (Product item in product)
            {
                list.Add(new ProductViewIndex
                {
                    ProductName = item.ProductName,
                    ProductPrice = item.Price,
                    ProductQuantity = item.Quantity
                });
            }
            return list;

        }

    }
}