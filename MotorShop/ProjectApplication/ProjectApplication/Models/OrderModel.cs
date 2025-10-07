using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class OrderModel
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public List<CartItemModel> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }
}