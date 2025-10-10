using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ProjectApplication.Models
{
    public class Vehicle : Product
    {
        public int ProductId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleName { get; set; }
        public int Engine {  get; set; }
        public string VehicleType { get; set; }
        public int FuelCapacity { get; set; }
        public string VehicleDescription { get; set; }
        public vehicleColor Color { get; set; }

        //Nhiều spare phù hợp với xe...
        public virtual ICollection<SparePart> spareParts { get; set; }


    }
}