// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.AdminPortal.ViewModels;

/// <summary>
/// View model for coupons.
/// </summary>
public class CouponViewModel : Coupon
{
    public Coupon ToModel()
    {
        return new Coupon() {
            Id = Id,
            Code = Code.Trim(),
            Discount = Discount,
            IsDiscountFixed = IsDiscountFixed
        };
    }

    public static CouponViewModel CreateDefault()
    {
        var couponViewModel = new CouponViewModel() {
            Code = "",
            Discount = 0,
            IsDiscountFixed = true
        };

        return couponViewModel;
    }

    public static CouponViewModel FromModel(Coupon model)
    {
        var couponViewModel = CreateDefault();
        couponViewModel.Id = model.Id;
        couponViewModel.Code = model.Code;
        couponViewModel.Discount = model.Discount;
        couponViewModel.IsDiscountFixed = model.IsDiscountFixed;
        return couponViewModel;
    }

    public bool Equals(Coupon model) => Id == model.Id;
}