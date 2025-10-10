using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class SparePart : Product
    {
        public int SparePartId { get; set; }
        public int ProductId { get; set; }
        public string SparePartName { get; set; }

        public string SparePartDescription { get; set; }

        // Danh sách các xe phù hợp với phụ kiện
        public virtual ICollection<Vehicle> vehicle { get; set; }

    }
}