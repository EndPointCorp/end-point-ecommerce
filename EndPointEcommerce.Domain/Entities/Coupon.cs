// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.Domain.Entities;

/// <summary>
/// Entity for coupons.
/// </summary>
[Index(nameof(Code), IsUnique = true)]
public class Coupon : BaseAuditEntity
{
    [UniqueCouponCode]
    public required string Code { get; set; }
    [Display(Name = "Discount amount/percentage")]
    public required decimal Discount { get; set; }
    [Display(Name = "Is fixed amount?")]
    public required bool IsDiscountFixed { get; set; }

    public bool CodeIs(string code) =>
        string.Equals(Code, code, StringComparison.InvariantCultureIgnoreCase);
}
