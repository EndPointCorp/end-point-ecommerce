// Copyright 2025 End Point Corporation. Apache License, version 2.0.

namespace EndPointEcommerce.WebApi.ResourceModels;

public class Coupon
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal? Discount { get; set; }
    public bool? IsDiscountFixed { get; set; }

    public static Coupon FromEntity(Domain.Entities.Coupon entity)
    {
        return new() {
            Id = entity.Id,
            Code = entity.Code,
            Discount = entity.Discount,
            IsDiscountFixed = entity.IsDiscountFixed
        };
    }
}