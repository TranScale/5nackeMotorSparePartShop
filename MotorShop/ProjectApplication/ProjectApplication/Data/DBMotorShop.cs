using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace ProjectApplication.Models
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext() : base("name=ShopConnection")
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<SparePart> SpareParts { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Product>()
            //    .Map<Vehicle>(m => m.Requires("TypeProduct").HasValue("1"))
            //    .Map<SparePart>(m => m.Requires("TypeProduct").HasValue("2"));

            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Vehicle>().ToTable("Vehicles");
            modelBuilder.Entity<SparePart>().ToTable("SpareParts");

            // many-to-many mapping stays the same (EF will generate VehicleSparePart table)
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.CompatibleSpareParts)
                .WithMany(s => s.SuitableVehicles)
                .Map(cs =>
                {
                    cs.MapLeftKey("VehicleId");
                    cs.MapRightKey("SparePartId");
                    cs.ToTable("VehicleSparePart");
                });
        }


    }
}