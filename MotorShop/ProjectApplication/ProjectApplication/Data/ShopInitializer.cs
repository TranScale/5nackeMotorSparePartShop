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
            // ===== 1️⃣ Admin =====
            var admins = new List<Admin>
            {
                new Admin
                {
                    AdminName = "admin",
                    AdminPassword = "123456",
                    AdminEmail = "admin@motorstore.com",
                    AdminPhone = "0123456789",
                    AdminAddress = "Hanoi"
                },
            };
            admins.ForEach(a => context.Admins.Add(a));
            context.SaveChanges();

            // ===== 2️⃣ Vehicles =====
            var vehicles = new List<Vehicle>
            {
                new Vehicle
                {
                    TypeProduct = "1",
                    vehicleName = "Yamaha Exciter 155",
                    typeVehicle = "Sport",
                    Displacement = 155,
                    fuelCapacity = 5.4f,
                    weight = 121,
                    Color = vehicleColor.Blue,
                    description = "Xe côn tay mạnh mẽ, phong cách thể thao.",
                    BrandId = 1,
                    price = 49000000,
                    number = 10  // số lượng tồn kho
                },
                new Vehicle
                {
                    TypeProduct = "1",
                    vehicleName = "Honda Vision 110",
                    typeVehicle = "Scooter",
                    Displacement = 110,
                    fuelCapacity = 4.9f,
                    weight = 97,
                    Color = vehicleColor.Red,
                    description = "Xe tay ga tiết kiệm xăng, phù hợp cho nữ giới.",
                    BrandId = 2,
                    price = 34000000,
                    number = 15
                }
            };
            vehicles.ForEach(v => context.Vehicles.Add(v));
            context.SaveChanges();

            // ===== 3️⃣ SpareParts =====
            var spareParts = new List<SparePart>
            {
                new SparePart
                {
                    TypeProduct = "2",
                    spareName = "Lọc nhớt Exciter 155",
                    spareDescription = "Lọc nhớt chính hãng Yamaha",
                    BrandId = 1,
                    price = 120000,
                    number = 50
                },
                new SparePart
                {
                    TypeProduct = "2",
                    spareName = "Lốp xe Vision 110",
                    spareDescription = "Lốp Michelin 80/90-14 dành cho Vision",
                    BrandId = 2,
                    price = 550000,
                    number = 30
                }
            };
            spareParts.ForEach(s => context.SpareParts.Add(s));
            context.SaveChanges();

            // ===== 4️⃣ Gán SparePart cho Vehicle =====
            vehicles[0].CompatibleSpareParts = new List<SparePart> { spareParts[0] };
            vehicles[1].CompatibleSpareParts = new List<SparePart> { spareParts[1] };

            context.SaveChanges();
        }
    }
}
