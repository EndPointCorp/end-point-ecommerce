using EndPointCommerce.AdminPortal.ViewModels;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.AdminPortal.Pages;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly EndPointCommerceDbContext _context;

    public AdminDashboardViewModel Dashboard { get; set; } = default!;
    public IList<Order> Orders { get; set; } = default!;

    public IndexModel(ILogger<IndexModel> logger, IOrderRepository orderRepository,
        ICustomerRepository customerRepository, IProductRepository productRepository,
        EndPointCommerceDbContext context)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Dashboard = new AdminDashboardViewModel();

        Dashboard.TodaysSales = await _orderRepository.GetTodaysSales();
        Dashboard.MonthSales = await _orderRepository.GetMonthSales();
        Dashboard.MonthCustomers = await _customerRepository.GetMonthCustomersCount();
        Dashboard.ActiveProducts = await _productRepository.GetActiveProductCount();

        var orders = await _orderRepository.FetchOrderCountsFromLastSevenDaysAsync();
        Dashboard.LoadOrdersChartFromGenericCountList(orders);

        var amounts = await _orderRepository.FetchOrderAmountsFromLastSevenDaysAsync();
        Dashboard.LoadAmountsChartFromGenericCountList(amounts);

        Orders = await _context.Orders.Include(x => x.Status).Include(x => x.Customer).
            Include(x => x.BillingAddress).ThenInclude(x => x.State).
            OrderByDescending(x => x.Id).Take(10).ToListAsync();

        return Page();
    }
}
