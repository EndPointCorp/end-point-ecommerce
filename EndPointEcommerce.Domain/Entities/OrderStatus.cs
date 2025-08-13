using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.Domain.Entities;

/// <summary>
/// Entity for order status.
/// </summary>
public class OrderStatus : BaseEntity
{
    public const string PENDING = "Pending";
    public const string PROCESSING = "Processing";
    public const string INVOICED = "Invoiced";
    public const string CANCELLED = "Cancelled";

    [Display(Name = "Name")]
    public required string Name { get; set; }
}