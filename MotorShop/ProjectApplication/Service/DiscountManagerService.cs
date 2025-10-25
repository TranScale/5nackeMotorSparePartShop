using ProjectApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Service
{
    public class DiscountManagerService
    {
        //Tìm kiếm loại discount 
        public static List<Discount> SearchDiscountTypeList(string type)
        {
            // Nếu type null hoặc rỗng => trả về danh sách rỗng luôn
            if (string.IsNullOrWhiteSpace(type))
                return new List<Discount>();

            using (var db = new ShopDbContext())
            {
                var discounts = db.Discounts.ToList();
                var list = new List<Discount>();

                foreach (var discount in discounts)
                {
                    if (discount.DiscountType == type)
                        list.Add(discount);
                }

                // Nếu không tìm thấy => trả về danh sách rỗng
                return list;
            }
        }


        //Trạng thái của Discount 
        public static bool IsActive(DateTime dateStart, DateTime dateEnd)
        {
            DateTime now = DateTime.Now;
            return now >= dateStart && now <= dateEnd;
        }

        //Lấy Coupon 
        public static Coupon GetCoupon (DiscountViewDetails viewModel)
        {
            var coupon = new Coupon();
            coupon.DiscountId = viewModel.Id;
            coupon.DiscountName = viewModel.Name;
            coupon.DateStart = viewModel.DateStart;
            coupon.DateEnd = viewModel.DateEnd;
            coupon.DiscountType = viewModel.DiscountType;
            coupon.DiscountValueType = viewModel.DiscountValueType;
            coupon.DiscountValue = viewModel.DiscountValue;
            coupon.CouponCode = viewModel.CouponCode;
            coupon.isActive = DiscountManagerService.IsActive(coupon.DateStart, coupon.DateEnd);
            return coupon;
        }

        //Lấy Promotion
        public static Promotion GetPromotion(DiscountViewDetails viewModel)
        {
            var promotion = new Promotion();
            promotion.DiscountId = viewModel.Id;
            promotion.DiscountName = viewModel.Name;
            promotion.DateStart = viewModel.DateStart;
            promotion.DateEnd = viewModel.DateEnd;
            promotion.DiscountType = viewModel.DiscountType;
            promotion.DiscountValueType = viewModel.DiscountValueType;
            promotion.DiscountValue = viewModel.DiscountValue;
            promotion.PromotionDescription = viewModel.PromotionDescription;
            promotion.Condition = viewModel.DiscountCondition;
            promotion.isActive = DiscountManagerService.IsActive(promotion.DateStart, promotion.DateEnd);
            promotion.ImagePath = viewModel.ImagePath;
            return promotion;
        }


        //Update Coupon 
        public static void UpdateCouponFromViewModel(Coupon coupon, DiscountViewDetails viewModel)
        {
            // Chỉ cần gán, không tạo mới
            coupon.DiscountName = viewModel.Name;
            coupon.DateStart = viewModel.DateStart;
            coupon.DateEnd = viewModel.DateEnd;
            coupon.DiscountType = viewModel.DiscountType;
            coupon.DiscountValueType = viewModel.DiscountValueType;
            coupon.DiscountValue = viewModel.DiscountValue;
            coupon.CouponCode = viewModel.CouponCode;
        }

        //Update Promotion
        public static void UpdatePromotionFromViewModel(Promotion promotion, DiscountViewDetails viewModel)
        {
            // Chỉ cần gán, không tạo mới
            promotion.DiscountName = viewModel.Name;
            promotion.DateStart = viewModel.DateStart;
            promotion.DateEnd = viewModel.DateEnd;
            promotion.DiscountType = viewModel.DiscountType;
            promotion.DiscountValueType = viewModel.DiscountValueType;
            promotion.DiscountValue = viewModel.DiscountValue;
            promotion.Condition= viewModel.DiscountCondition;
            promotion.PromotionDescription= viewModel.PromotionDescription;
        }

    }
}