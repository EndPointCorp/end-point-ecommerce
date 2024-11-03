using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EndPointCommerce.Infrastructure.Repositories;

/// <summary>
/// Repository class for a Payment Method entity.
/// </summary>
public class PaymentMethodRepository : BaseRepository<PaymentMethod>, IPaymentMethodRepository
{
    public PaymentMethodRepository(EndPointCommerceDbContext context) : base(context) { }

    /// <summary>
    /// Retrieve the sorted, active list of payment methods
    /// </summary>
    public async Task<IEnumerable<PaymentMethod>> FetchAllAsync()
    {
        return await DbSet().OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<PaymentMethod?> FindByNameAsync(string name) =>
        await DbSet().FirstOrDefaultAsync(c => c.Name == name);

    public async Task<PaymentMethod> GetFreeOrder() =>
        await FindByNameAsync(PaymentMethod.FREE_ORDER) ??
            throw new InvalidOperationException("The database is not properly configured.");

    public async Task<PaymentMethod> GetCreditCard() =>
        await FindByNameAsync(PaymentMethod.CREDIT_CARD) ??
            throw new InvalidOperationException("The database is not properly configured.");
}