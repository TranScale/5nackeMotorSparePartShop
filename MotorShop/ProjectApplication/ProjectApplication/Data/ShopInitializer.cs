using System;
using System.Collections.Generic;
using System.Data.Entity;
using ProjectApplication.Models;

namespace ProjectApplication.Data
{
    public class ShopInitializer : DropCreateDatabaseIfModelChanges<ShopDbContext>
    {
        protected override void Seed(ShopDbContext context){}
    }
}
