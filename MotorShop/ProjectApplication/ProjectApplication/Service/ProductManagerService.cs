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

    }
}