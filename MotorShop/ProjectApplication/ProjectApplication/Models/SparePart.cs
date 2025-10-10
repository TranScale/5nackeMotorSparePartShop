using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class SparePart : Product
    {
        public int SparePartId { get; set; } //Khóa chính của SparePart 
        // Danh sách các xe phù hợp với phụ kiện
        public virtual ICollection<Vehicle> Vehicle { get; set; }

    }
}