// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.Domain.Entities;

/// <summary>
/// Entity for states.
/// </summary>
[Index(nameof(Abbreviation), IsUnique = true)]
public class State : BaseEntity
{
    [Display(Name = "State")]
    public required string Name { get; set; }
    public required string Abbreviation { get; set; }
}