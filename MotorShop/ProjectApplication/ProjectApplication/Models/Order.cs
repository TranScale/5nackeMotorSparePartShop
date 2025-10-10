using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    // Trong thư mục Models/
    public class Order
    {
        public int OrderId { get; set; }

        // Thông tin Khách hàng
        public string CustomerName { get; set; }
        public string Phone { get; set; }

        // Thông tin Địa chỉ
        public string Province { get; set; } // Tên Tỉnh/Thành
        public string District { get; set; } // Tên Quận/Huyện (Nhập tay)
        public string Ward { get; set; }     // Tên Phường/Xã (Nhập tay)
        public string AddressDetail { get; set; } // Địa chỉ chi tiết
        public string Notes { get; set; }

        // Thông tin Đơn hàng
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // Ví dụ: "Pending", "Processing", "Delivered"

        // Liên kết với OrderDetails
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}