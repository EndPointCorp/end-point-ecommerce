using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Tests.Domain.Entities;

public class QuoteItemTest
{
    [Fact]
    public void UnitPrice_ShouldReturnProductActualPrice()
    {
        // Arrange
        var quoteItem = new QuoteItem
        {
            Product = new Product
            {
                Name = "test_name",
                Sku = "test_sku",
                BasePrice = 100M,
            }
        };

        // Assert
        Assert.Equal(100M, quoteItem.UnitPrice);
    }

    [Fact]
    public void TotalPrice_ShouldReturnProductUnitPriceTimesQuantity()
    {
        // Arrange
        var quoteItem = new QuoteItem
        {
            Quantity = 2,
            Product = new Product
            {
                Name = "test_name",
                Sku = "test_sku",
                BasePrice = 100M,
            }
        };

        // Assert
        Assert.Equal(200m, quoteItem.TotalPrice);
    }

    [Fact]
    public void Discount_ShouldCallOnTheQuoteToCalculate()
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

        // Assert
        Assert.Equal(20M, quoteItem.Discount);
    }

    [Fact]
    public void Total_ShouldReturnTotalPriceMinusDiscount()
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

        // Assert
        Assert.Equal(180, quoteItem.Total);
    }

    [Fact]
    public void Build_ShouldCreateQuoteItemWithCorrectProperties()
    {
        // Arrange
        var quote = new Quote { Id = 1 };
        var product = new Product
        {
            Id = 1,
            Name = "test_name",
            Sku = "test_sku",
            BasePrice = 100M
        };
        int quantity = 5;

        // Act
        var quoteItem = QuoteItem.Build(quote, product, quantity);

        // Assert
        Assert.Equal(quote.Id, quoteItem.QuoteId);
        Assert.Equal(quote, quoteItem.Quote);
        Assert.Equal(product.Id, quoteItem.ProductId);
        Assert.Equal(product, quoteItem.Product);
        Assert.Equal(quantity, quoteItem.Quantity);
    }

    [Fact]
    public void Clone_ShouldCreateANewObjectWithTheSameProperties()
    {
        // Arrange
        var quoteItem = new QuoteItem
        {
            Quantity = 2,
            ProductId = 1,
            Product = new Product
            {
                Name = "test_name",
                Sku = "test_sku",
                BasePrice = 100M,
            }
        };

        // Act
        var clonedQuoteItem = quoteItem.Clone();

        // Assert
        Assert.Equal(quoteItem.ProductId, clonedQuoteItem.ProductId);
        Assert.Equal(quoteItem.Product, clonedQuoteItem.Product);
        Assert.Equal(quoteItem.Quantity, clonedQuoteItem.Quantity);
        Assert.NotSame(quoteItem, clonedQuoteItem);
    }
}
