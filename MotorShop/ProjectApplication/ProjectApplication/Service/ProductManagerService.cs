using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectApplication.Models;
using System.Data.Entity;
using System.Diagnostics;


namespace ProjectApplication.Service
{
    public class ProductManagerService
    {
        //Tìm kiếm thể loại Product (ProductType)
        public static List<Product> SearchProductType(string type)
        {
            ShopDbContext db = new ShopDbContext();
            var products = db.Products.ToList();
            var list = new List<Product>();
            foreach (var product in products)
            {
                if(product.ProductType == type)
                    { list.Add(product); }
            }
            if(list.Count == 0) 
                return products;
            return list;
        }

        //Tìm kiếm theo chuỗi string 
        public static List<Product> SearchProductString(string key)
        {
            ShopDbContext db = new ShopDbContext();
            var products = db.Products.ToList();
            var list = new List<Product>();
            foreach (var product in products)
            {
                if (product.ProductName.ToLower().Contains(key.ToLower())) { list.Add(product); }
            }
            return list;
        }

        public static SparePart GetSparePart(ProductViewDetail viewModel)
        {
            var part = new SparePart();
            part.ProductId = viewModel.ProductId;
            part.ProductName = viewModel.ProductName;
            part.ProductType = viewModel.ProductType;
            part.Price = viewModel.ProductPrice;
            part.BrandId = viewModel.BrandId;
            part.Quantity = viewModel.ProductQuantity;
            part.ProductDescription = viewModel.ProductDescription;

            return part;
        }

        //Update SparePart
        public static void UpdateSparePart(ProductViewDetail viewModel, SparePart part)
        {
            part.ProductId = viewModel.ProductId;
            part.ProductName = viewModel.ProductName;
            part.ProductType = viewModel.ProductType;
            part.Price = viewModel.ProductPrice;
            part.BrandId = viewModel.BrandId;
            part.Quantity = viewModel.ProductQuantity;
            part.ProductDescription = viewModel.ProductDescription;

        }

        public static Vehicle GetVehicle(ProductViewDetail viewModel)
        {
            var vehicle = new Vehicle();
            vehicle.ProductId = viewModel.ProductId;
            vehicle.ProductName = viewModel.ProductName;
            vehicle.ProductType = viewModel.ProductType;
            vehicle.Price = viewModel.ProductPrice;
            vehicle.BrandId = viewModel.BrandId;
            vehicle.Quantity = viewModel.ProductQuantity;
            vehicle.ProductDescription = viewModel.ProductDescription;
            vehicle.Engine = viewModel.Engine;
            vehicle.VehicleType = viewModel.VehicleType;
            vehicle.FuelCapacity = viewModel.FuelCapacity;
            vehicle.Color = viewModel.Color;

            return vehicle;
        }

        //Update Vehicle
        public static void UpdateVehicle(ProductViewDetail viewModel, Vehicle vehicle)
        {
            vehicle.ProductId = viewModel.ProductId;
            vehicle.ProductName = viewModel.ProductName;
            vehicle.ProductType = viewModel.ProductType;
            vehicle.Price = viewModel.ProductPrice;
            vehicle.BrandId = viewModel.BrandId;
            vehicle.Quantity = viewModel.ProductQuantity;
            vehicle.ProductDescription = viewModel.ProductDescription;
            vehicle.Engine = viewModel.Engine;
            vehicle.VehicleType = viewModel.VehicleType;
            vehicle.FuelCapacity = viewModel.FuelCapacity;
            vehicle.Color = viewModel.Color;

        }

        //Lấy Product từ ViewModel 
        public static Product GetProduct(ProductViewDetail viewModel)
        {
            return new Product
            {
                ProductId = viewModel.ProductId,
                ProductName = viewModel.ProductName,
                ProductType = viewModel.ProductType,
                Price = viewModel.ProductPrice,
                BrandId = viewModel.BrandId,
                Quantity = viewModel.ProductQuantity, 
                ProductDescription = viewModel.ProductDescription
            };
        }
        //Nhập hàng 
        public static void AddItem(int id, int quantity, ShopDbContext db)
        {
            var product = db.Products.Find(id);
            if (product == null)
                throw new Exception("Không tìm thấy sản phẩm.");

            var order = new Order
            {
                CustomerName = "Admin",
                OrderDate = DateTime.Now,
                TotalAmount = -(product.Price * quantity),
                Status = "Pending" // hoặc Pending nếu bạn muốn duyệt trước
            };
            db.Orders.Add(order);
            db.SaveChanges();

            var orderDetail = new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Quantity = quantity,
                Price = product.Price
            };

            db.OrderDetails.Add(orderDetail);

            db.SaveChanges();
        }

        //Đánh dấu là đã giao hàng 
        public static void Restock(int id, ShopDbContext db)
        {
            var order = db.Orders.Find(id);
            if (order == null)
                throw new Exception("Không thấy đơn hàng nha bro !!!");

            if(order.CustomerName == "Admin" && order.Status == "Delivered")
            {
                var items = db.OrderDetails.Where(i => i.OrderId == order.OrderId).ToList();
                foreach (var item in items)
                {
                    var product = db.Products.Find(item.ProductId);
                    if (product != null)
                    {
                        product.Quantity += item.Quantity; // ✅ Cộng số lượng vào kho
                    }
                }
                db.SaveChanges();
            }
        }

        

    }
}