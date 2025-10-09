using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectApplication.Models
{
    public class DiscountViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Tên khuyến mãi")]
        public string Name { get; set; }
        [Display(Name = "Ngày bắt đầu")]
        public DateTime dateStart { get; set; }
        [Display(Name = "Ngày kết thúc")]
        public DateTime dateEnd { get; set; }
        [Display(Name = "Loại khuyến mãi")]
        public DiscountType discountType { get; set; }
        [Display(Name = "Kiểu giá trị khuyến mãi")]
        public DiscountValueType discountValueType { get; set; }
        [Display(Name = "Giá trị khuyến mãi")]
        public decimal discountValue { get; set; }
        [Display(Name = "Trạng thái")]
        public bool isActive { get; set; }

        //Thông tin của Coupon
        [Display(Name = "Mã")]
        public string couponCode { get; set; }

        //Thông tin của promotion
        [Display(Name = "Mô tả chương trình khuyến mãi")]
        public string promotionDescription { get; set; }

        public void SetDiscountViewModel(Basediscount discount)
        {
            Id = discount.BasediscountId;
            Name = discount.BasediscountName;
            discountValueType = discount.discountValueType;
            discountType = discount.discountType;
            discountValue = discount.discountValue;
            dateStart = discount.dateStart;
            dateEnd = discount.dateEnd;
            isActive = discount.isActive;
        }

    }
}