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

    }
}