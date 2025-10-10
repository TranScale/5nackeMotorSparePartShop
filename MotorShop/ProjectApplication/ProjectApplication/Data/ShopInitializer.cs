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
            //2 hãng (Brand)
            var brand = new List<Brand>
            {
                new Brand { BrandName = "Yamaha", BrandDescription = "Hãng xe nổi tiếng từ Nhật Bản" },
                new Brand { BrandName = "Honda", BrandDescription = "Thương hiệu phổ biến nhất Việt Nam" },
                new Brand { BrandName = "Suzuki", BrandDescription = "Hãng xe Nhật Bản, nổi bật với xe số và xe tay ga" },
                new Brand { BrandName = "Kawasaki", BrandDescription = "Hãng xe mô tô thể thao, mạnh mẽ" },
                new Brand { BrandName = "Motul", BrandDescription = "Hãng dầu nhớt nổi tiếng, chất lượng cao" },
                new Brand { BrandName = "RacingBoy", BrandDescription = "Phụ tùng hiệu suất cao, dành cho xe đua" },
                new Brand { BrandName = "NGK", BrandDescription = "Nhà sản xuất bugi nổi tiếng của Nhật Bản" },
                new Brand { BrandName = "GS", BrandDescription = "Ắc quy xe máy chính hãng, bền bỉ" },
            };
            brand.ForEach(b => context.Brands.Add(b));
            context.SaveChanges();

            //5 chiếc xe (Vehicle)
            var vehicle = new List<Vehicle>
            {
                new Vehicle { ProductName = "Wave Alpha", BrandId = 1, ProductType = "Vehicle", Price = 18000000, Quantity = 10, ProductDescription = "Xe số tiết kiệm xăng", Engine = 110, VehicleType = "Xe số", FuelCapacity = 4.0f, Color = vehicleColor.Red },
                new Vehicle { ProductName = "Vision", BrandId = 2, ProductType = "Vehicle", Price = 32000000, Quantity = 7, ProductDescription = "Xe tay ga phổ thông", Engine = 125, VehicleType = "Xe tay ga", FuelCapacity = 5.2f, Color = vehicleColor.Blue },
                new Vehicle { ProductName = "Winner X", BrandId = 1, ProductType = "Vehicle", Price = 46000000, Quantity = 4, ProductDescription = "Xe côn tay mạnh mẽ", Engine = 150, VehicleType = "Xe côn tay", FuelCapacity = 4.5f, Color = vehicleColor.Black },
                new Vehicle { ProductName = "Air Blade", BrandId = 2, ProductType = "Vehicle", Price = 42000000, Quantity = 5, ProductDescription = "Xe tay ga thể thao", Engine = 125, VehicleType = "Xe tay ga", FuelCapacity = 5.0f, Color = vehicleColor.White },
                new Vehicle { ProductName = "SH Mode", BrandId = 3, ProductType = "Vehicle", Price = 62000000, Quantity = 3, ProductDescription = "Xe tay ga cao cấp", Engine = 150, VehicleType = "Xe tay ga", FuelCapacity = 5.5f, Color = vehicleColor.Gray },
            };
            vehicle.ForEach(v => context.Vehicles.Add(v));

            //5 phụ tùng (Spare Part)
            var sparepart = new List<SparePart>
            {
                new SparePart {ProductName = "Nhớt Motul 3100", BrandId = 4, ProductType = "SparePart", Price = 150000, Quantity = 50, ProductDescription = "Nhớt dành cho xe số và tay ga"},
                new SparePart {ProductName = "Bộ phanh đĩa RacingBoy", BrandId = 5, ProductType = "SparePart", Price = 850000, Quantity = 15, ProductDescription = "Phanh đĩa hiệu suất cao"},
                new SparePart {ProductName = "Lọc gió K&N", BrandId = 6, ProductType = "SparePart", Price = 300000, Quantity = 20, ProductDescription = "Lọc gió cho xe tay ga"},
                new SparePart { ProductName = "Bugi NGK Iridium", BrandId = 7, ProductType = "SparePart", Price = 120000, Quantity = 40, ProductDescription = "Bugi bền, đánh lửa mạnh" },
                new SparePart { ProductName = "Ắc quy GS", BrandId = 8, ProductType = "SparePart", Price = 400000, Quantity = 25, ProductDescription = "Ắc quy xe máy chính hãng" },
            };
            sparepart.ForEach(sp => context.SpareParts.Add(sp));

            context.SaveChanges();

            base.Seed(context);
        }
    }
}
