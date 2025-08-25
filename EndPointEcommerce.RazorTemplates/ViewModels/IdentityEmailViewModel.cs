// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.RazorTemplates.ViewModels;

public class IdentityEmailViewModel
{
    public required User User { get; set; }
    public required string Link { get; set; }
}
