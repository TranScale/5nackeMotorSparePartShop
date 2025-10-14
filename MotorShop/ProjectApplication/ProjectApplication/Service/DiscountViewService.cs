using ProjectApplication.Controllers;
using ProjectApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectApplication.Service
{
    public class DiscountViewService
    {
        //-------------------------------ALL------------------------------------------
        public static DiscountViewDetails returnCoupon(Coupon coupon, DiscountViewDetails viewModel)
        {
            viewModel.CouponCode = coupon.CouponCode;
            return viewModel;
        }

        public static DiscountViewDetails returnPromotion(Promotion promotion, DiscountViewDetails viewModel)
        {
            viewModel.PromotionDescription = promotion.PromotionDescription;
            viewModel.DiscountCondition = promotion.Condition;
            return viewModel;
        }

        //Xuất 1 trong Index
        public static DiscountViewIndex GetDiscount (Discount discount)
        {
            return new DiscountViewIndex
            {
                Id = discount.DiscountId,
                Name = discount.DiscountName,
                DateStart = discount.DateStart,
                DateEnd = discount.DateEnd,
                DiscountType = discount.DiscountType,
                DiscountValueType = discount.DiscountValueType,
                DiscountValue = discount.DiscountValue,
                IsActive = DiscountManagerService.IsActive(discount.DateStart, discount.DateEnd)
            };
        }

        //Xuất list trong Index 
        public static List<DiscountViewIndex> GetListDiscount(List<Discount> discounts)
        {
            var list = new List<DiscountViewIndex>();
            foreach (Discount item in discounts)
            {
                list.Add(new DiscountViewIndex
                {
                    Id = item.DiscountId,
                    Name = item.DiscountName,
                    DateStart = item.DateStart,
                    DateEnd = item.DateEnd,
                    DiscountType = item.DiscountType,
                    DiscountValueType = item.DiscountValueType,
                    DiscountValue = item.DiscountValue,
                    IsActive = DiscountManagerService.IsActive(item.DateStart, item.DateEnd)
                });
            }
            return list;
        }

        // View detail 
        public static DiscountViewDetails GetViewDetail (Discount discount)
        {
            var viewModel = new DiscountViewDetails
            {
                Id = discount.DiscountId,
                Name = discount.DiscountName,
                DateStart = discount.DateStart,
                DateEnd = discount.DateEnd,
                DiscountType = discount.DiscountType,
                DiscountValueType = discount.DiscountValueType,
                DiscountValue = discount.DiscountValue,
                IsActive = DiscountManagerService.IsActive(discount.DateStart, discount.DateEnd)
            };
            if(discount is Coupon coupon)
            {
                returnCoupon(coupon, viewModel);
                return viewModel;
            }
            else if(discount is Promotion promotion)
            {
                returnPromotion(promotion, viewModel);
                return viewModel;
            }
            else
                throw new Exception("Discount not found");
        }



    }
}