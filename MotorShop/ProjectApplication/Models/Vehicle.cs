using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ProjectApplication.Models
{
    public class Vehicle : Product
    {
        public int VehicleId { get; set; } //Khóa chính của xe 
        public int Engine {  get; set; } //Động cơ xe (vd: 110cc, 125cc,150cc,...)
        public string VehicleType { get; set; } //Loại xe (vd: Xe số, xe tay ga, Xe côn)
        public float FuelCapacity { get; set; } //Dung tích xăng (vd: 4.5l,5.5l,...)
        public vehicleColor Color { get; set; } //Màu của xe

        //các spare phù hợp với xe...
        public virtual ICollection<SparePart> spareParts { get; set; }


    }
}