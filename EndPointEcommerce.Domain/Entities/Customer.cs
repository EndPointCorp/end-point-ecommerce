// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.Domain.Entities;

/// <summary>
/// Entity for customers.
/// </summary>
public class Customer: BaseAuditEntity
{
    [Display(Name = "First Name")]
    public required string Name { get; set; }
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }
    [Display(Name = "Email")]
    public required string Email { get; set; }
    [Display(Name = "Full Name")]
    public string FullName => LastName == null ? Name : $"{Name} {LastName}";
}