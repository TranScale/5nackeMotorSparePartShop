using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ProjectApplication.Models
{
    public class Vehicle : Product
    {
        //public String TypeProduct { get; set; } = "1";
        [Display(Name = "Tên xe")]
        public String vehicleName {  get; set; }
        [Display(Name = "Loại xe")]
        public String typeVehicle {  get; set; }
        [Display(Name = "Phân khối")]
        public int Displacement { get; set; }
        [Display(Name = "Dung tích xăng")]
        public float fuelCapacity { get; set; }
        [Display(Name = "Trọng lượng")]
        public int weight { get; set; }
        [Display(Name = "Màu sắc")]
        public vehicleColor Color { get; set; }
        [Display(Name = "Miêu tả")]
        public String description { get; set; }
        public virtual ICollection<SparePart> CompatibleSpareParts { get; set; }
    }
}