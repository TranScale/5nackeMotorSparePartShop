using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class OrderViewIndex
    {
        public int Id { get; set; } //Set primary key
        public string Name { get; set; } //Name customer
        public string Phone { get; set; } //Customer's phone number
        public DateTime OrderDate { get; set; } // Date Customer place order
        public string OrderStatus { get; set; } // "Pending","Processing" and "Delivered"
        public decimal TotalAmount { get; set; } //Price
    }
}