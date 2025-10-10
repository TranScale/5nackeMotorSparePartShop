using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class Vehicle : Product
    {
        public String TypeProduct { get; set; } = "1";
        public String vehicleName { get; set; }
        public String typeVehicle { get; set; }
        public int Displacement { get; set; }
        public float fuelCapacity { get; set; }
        public int weight { get; set; }
        public vehicleColor Color { get; set; }
        public String description { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public virtual ICollection<SparePart> CompatibleSpareParts { get; set; }
    }
}