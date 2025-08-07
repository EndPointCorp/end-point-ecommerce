using static EndPointEcommerce.Domain.Interfaces.IOrderRepository;

namespace EndPointEcommerce.AdminPortal.ViewModels;

/// <summary>
/// View model for the dashboard.
/// </summary>
public class AdminDashboardViewModel
{
    public decimal TodaysSales { get; set; } = 0.0M;
    public decimal MonthSales { get; set; } = 0.0M;
    public int MonthCustomers { get; set; } = 0;
    public int ActiveProducts { get; set; } = 0;

    public IList<ChartDashboardViewModel> OrdersChart { get; set; } = new List<ChartDashboardViewModel>();
    public IList<ChartDashboardViewModel> AmountsChart { get; set; } = new List<ChartDashboardViewModel>();

    public void LoadOrdersChartFromGenericCountList(IList<CountPerGroup> list)
    {
        OrdersChart.Clear();
        foreach (var item in list)
        {
            OrdersChart.Add(new ChartDashboardViewModel
            {
                Label = item.Group,
                Value = item.Value,
            });
        }
    }

    public void LoadAmountsChartFromGenericCountList(IList<CountPerGroup> list)
    {
        AmountsChart.Clear();
        foreach (var item in list)
        {
            AmountsChart.Add(new ChartDashboardViewModel
            {
                Label = item.Group,
                Value = item.Value,
            });
        }
    }

}