using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class ProductViewModel
    {
        // Thuộc tính chung Product
        public int Id { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }

        public int BrandId { get; set; }
        // Phân biệt loại
        public int number { get; set; }
        public string ProductType { get; set; }
        public int Engine { get; set; }

        public virtual ICollection<Vehicle> CompatibleModel { get; set; }
        public String typeVehicle { get; set; }
        public float fuelCapacity { get; set; }
        public int weight { get; set; }
        public vehicleColor Color { get; set; }
        public String description { get; set; }
        public virtual ICollection<SparePart> CompatibleSpareParts { get; set; }

        public String spareDescription { get; set; }

        public String TypeProduct {  get; set; }
        public String BrandName { get; set; }

        public virtual ICollection<Vehicle> SuitableVehicles { get; set; }
    }

}