using System;
using System.Collections.Generic;
using System.Data.Entity;
using ProjectApplication.Models;

namespace ProjectApplication.Data
{
    public class ShopInitializer : DropCreateDatabaseIfModelChanges<ShopDbContext>
    {
        protected override void Seed(ShopDbContext context)
        {
            //5 chiếc xe
            var vehicle = new List<Vehicle>
            {
                new Vehicle { ProductName = "Wave Alpha", BrandId = 1, ProductType = "Vehicle", Price = 18000000, Quantity = 10, ProductDescription = "Xe số tiết kiệm xăng", Engine = 110, VehicleType = "Xe số", FuelCapacity = 4.0f, Color = vehicleColor.Red },
                new Vehicle { ProductName = "Vision", BrandId = 2, ProductType = "Vehicle", Price = 32000000, Quantity = 7, ProductDescription = "Xe tay ga phổ thông", Engine = 125, VehicleType = "Xe tay ga", FuelCapacity = 5.2f, Color = vehicleColor.Blue },
                new Vehicle { ProductName = "Winner X", BrandId = 1, ProductType = "Vehicle", Price = 46000000, Quantity = 4, ProductDescription = "Xe côn tay mạnh mẽ", Engine = 150, VehicleType = "Xe côn tay", FuelCapacity = 4.5f, Color = vehicleColor.Black },
                new Vehicle { ProductName = "Air Blade", BrandId = 2, ProductType = "Vehicle", Price = 42000000, Quantity = 5, ProductDescription = "Xe tay ga thể thao", Engine = 125, VehicleType = "Xe tay ga", FuelCapacity = 5.0f, Color = vehicleColor.White },
                new Vehicle { ProductName = "SH Mode", BrandId = 3, ProductType = "Vehicle", Price = 62000000, Quantity = 3, ProductDescription = "Xe tay ga cao cấp", Engine = 150, VehicleType = "Xe tay ga", FuelCapacity = 5.5f, Color = vehicleColor.Gray },
            };
            vehicle.ForEach(v => context.Vehicles.Add(v));
        }
    }
}
