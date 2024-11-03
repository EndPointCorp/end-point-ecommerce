using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.UnitTests.Domain.Entities;

public class OrderItemTests
{
    [Fact]
    public void Build_ShouldCreateANewOrderItemFromTheQuoteItem()
    {
        // Arrange
        var quoteItem = new QuoteItem
        {
            Quote = new Quote
            {
                Coupon = new Coupon
                {
                    Code = "test_code",
                    Discount = 10M,
                    IsDiscountFixed = true
                }
            },
            Quantity = 2,
            Product = new Product
            {
                Name = "test_name",
                Sku = "test_sku",
                BasePrice = 100M,
            }
        };

        // Act
        var orderItem = OrderItem.Build(quoteItem);

        // Assert
        Assert.Equal(quoteItem.Id, orderItem.QuoteItemId);
        Assert.Equal(quoteItem.ProductId, orderItem.ProductId);
        Assert.Equal(quoteItem.Quantity, orderItem.Quantity);
        Assert.Equal(quoteItem.UnitPrice, orderItem.UnitPrice);
        Assert.Equal(quoteItem.TotalPrice, orderItem.TotalPrice);
        Assert.Equal(quoteItem.Discount, orderItem.Discount);
        Assert.Equal(quoteItem.Total, orderItem.Total);
    }
}