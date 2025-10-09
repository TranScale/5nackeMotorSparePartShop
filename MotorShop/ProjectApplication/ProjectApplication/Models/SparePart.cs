using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class SparePart : Product
    {
        //public String TypeProduct { get; set; } = "2";

        [Display(Name = "Tên phụ kiện")]
        public String spareName { get; set; }
        [Display(Name = "Mô tả phụ kiện")]
        public String spareDescription { get; set; }

        public virtual ICollection<Vehicle> SuitableVehicles {  get; set; }
        //public virtual Product product { get; set; }

    }
}