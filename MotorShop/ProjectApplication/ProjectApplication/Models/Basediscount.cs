using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class Basediscount
    {
        [Key]
        public int BasediscountId { get; set; }
        public string BasediscountName { get; set; }

        public DiscountValueType discountValueType { get; set; }

        public DiscountType discountType { get; set; }
        public decimal discountValue { get; set; }

        [DataType(DataType.Date)]

        public DateTime dateStart { get; set; }
        [DataType(DataType.Date)]
        public DateTime dateEnd { get; set; } 
        public bool isActive {  get; set; }

        public virtual Coupon coupon { get; set; }
        public virtual Promotion Promotion { get; set; }

    }
}