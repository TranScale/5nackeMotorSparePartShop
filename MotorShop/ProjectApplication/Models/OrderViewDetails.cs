using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class OrderViewDetails
    {
        public int OrderId { get; set; } // Khóa chính
        public string CustomerName { get; set; } //Tên khách hàng
        public string Phone { get; set; } //Số điện thoại khách hàng
        public string Province { get; set; } // Địa chỉ (Tỉnh thành)
        public string District { get; set; } //Địa chỉ (quận)
        public string Ward { get; set; } //Địa chỉ (Phường)
        public string AddressDetail { get; set; } //Địa chỉ (số nhà)
        public string Notes { get; set; } //Ghi chú
        public DateTime OrderDate { get; set; } //Ngày đặt hàng
        public decimal TotalAmount { get; set; } //Tổng tiền
        public string Status { get; set; } //Trạng thái 

        public List<OrderDetailItem> Item { get; set; }

    }
    public class OrderDetailItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public decimal SubTotal => Quantity * Price; // Tính thành tiền từng sản phẩm
    }
}