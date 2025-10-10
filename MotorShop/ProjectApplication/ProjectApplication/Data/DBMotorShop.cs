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

        public DbSet<Admin> Admins { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<SparePart> SpareParts { get; set; }
        public DbSet<Brand> Brands { get; set; }

        public DbSet<Basediscount> Basediscounts { get; set; }
        public DbSet<Coupon> Coupons {  get; set; }
        public DbSet<Promotion> Promotions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

    }
}