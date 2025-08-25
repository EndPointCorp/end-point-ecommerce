// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.Domain.Entities;

/// <summary>
/// Entity for payment methods.
/// </summary>
public class PaymentMethod : BaseEntity
{
    public const string FREE_ORDER = "Free Order";
    public const string CREDIT_CARD = "Credit Card";

    [Display(Name = "Name")]
    public required string Name { get; set; }
}