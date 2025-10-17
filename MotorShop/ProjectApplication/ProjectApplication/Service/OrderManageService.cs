using ProjectApplication.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ProjectApplication.Service
{
    public class OrderManageService
    {
        public static Order AdminOrder(ProductViewDetail viewModel)
        {
            var order = new Order
            {
                CustomerName = "Admin",
                Phone = "0000000000",
                Province = "System",
                District = "-",
                Ward = "-",
                AddressDetail = "Kho hệ thống",
                Notes = "Tự động tạo khi thêm sản phẩm mới (Admin)",
                OrderDate = DateTime.Now,
                Status = "Pending",
                TotalAmount = -(viewModel.ProductPrice * viewModel.ProductQuantity)
            };
            return order;
        }

        public static OrderDetail GetOrderDetail(ProductViewDetail viewModel, int orderId, int productId)
        {
            var order = new OrderDetail();
            order.OrderId = orderId;
            order.ProductId = productId; // ⚡ Dùng ID thật của Product trong DB
            order.ProductName = viewModel.ProductName;
            order.Quantity = viewModel.ProductQuantity;
            order.Price = -viewModel.ProductPrice;

            return order;
        }


    }
}