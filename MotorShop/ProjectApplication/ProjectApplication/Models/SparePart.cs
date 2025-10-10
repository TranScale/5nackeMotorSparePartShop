using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class SparePart : Product
    {
        public int SparePartId { get; set; } //Khóa chính của SparePart 
        public string SparePartName { get; set; } //Tên phụ tùng 
        public string SparePartDescription { get; set; } //Mô tả phụ tùng 

        // Danh sách các xe phù hợp với phụ kiện
        public virtual ICollection<Vehicle> Vehicle { get; set; }

    }
}