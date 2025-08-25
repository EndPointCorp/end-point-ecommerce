// Copyright 2025 End Point Corporation. Apache License, version 2.0.

namespace EndPointEcommerce.AdminPortal.ViewModels;

/// <summary>
/// View model for a chart item in the dashboard.
/// </summary>
public class ChartDashboardViewModel
{
    public string Label { get; set; } = "";
    public decimal Value { get; set; } = 0.0M;
}