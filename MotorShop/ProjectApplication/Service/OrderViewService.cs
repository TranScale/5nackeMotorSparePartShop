using ProjectApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Service
{
    public class OrderViewService
    {

        public static List<OrderViewIndex> GetListIndexView(List<Order> orders)
        {
            var list = new List<OrderViewIndex>();
            foreach (Order item in orders)
            {
                list.Add(new OrderViewIndex
                {
                    Id = item.OrderId,
                    Name = item.CustomerName,
                    OrderDate = item.OrderDate,
                    OrderStatus = item.Status,
                    TotalAmount = item.TotalAmount,
                    Phone = item.Phone,
                });
            }
            return list;
        }

        public static OrderViewDetails GetDetails (Order order)
        {
            var detail = new OrderViewDetails
            {
                OrderId = order.OrderId,
                CustomerName = order.CustomerName,
                Phone = order.Phone,
                Province = order.Province,
                District = order.District,
                Ward = order.Ward,
                AddressDetail = order.AddressDetail,
                Notes = order.Notes,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Item = order.OrderDetails.Select(d => new OrderDetailItem
                {
                    ProductId = d.ProductId,
                    ProductName = d.ProductName,
                    Quantity = d.Quantity,
                    Price = d.Price
                }).ToList()
            };
            return detail;
        }

    }
}