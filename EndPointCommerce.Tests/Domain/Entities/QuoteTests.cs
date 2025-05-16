using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.Tests.Domain.Entities;

public class QuoteTests
{
    [Fact]
    public void Price_ShouldCalculateBasedOnTotalPriceOfItems()
    {
        // Arrange
        var quote = new Quote
        {
            Items =
            [
                new QuoteItem
                {
                    Quantity = 1,
                    Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 100 }
                },
                new QuoteItem
                {
                    Quantity = 2,
                    Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 200 }
                },
            ]
        };

        // Assert
        Assert.Equal(500, quote.Price);
    }

    [Fact]
    public void Price_ShouldReturnZero_WhenThereAreNoItems()
    {
        // Arrange
        var quote = new Quote();

        // Assert
        Assert.Equal(0, quote.Price);
    }

    [Fact]
    public void Discount_ShouldCalculateBasedOnDiscountOfItems()
    {
        // Arrange
        var quote = new Quote
        {
            Coupon = new Coupon { Code = "test_code", Discount = 10, IsDiscountFixed = true },
        };

        quote.Items = [
            new QuoteItem
            {
                Quote = quote,
                Quantity = 1,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 100 }
            },
            new QuoteItem
            {
                Quote = quote,
                Quantity = 2,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 200 }
            },
        ];

        // Assert
        Assert.Equal(30, quote.Discount);
    }

    [Fact]
    public void Discount_ShouldReturnZero_WhenThereIsNoCoupon()
    {
        // Arrange
        var quote = new Quote();

        quote.Items = [
            new QuoteItem
            {
                Quote = quote,
                Quantity = 1,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 100 }
            },
            new QuoteItem
            {
                Quote = quote,
                Quantity = 2,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 200 }
            },
        ];

        // Assert
        Assert.Equal(0, quote.Discount);
    }

    [Fact]
    public void Discount_ShouldReturnZero_WhenThereAreNoItems()
    {
        // Arrange
        var quote = new Quote();

        // Assert
        Assert.Equal(0, quote.Discount);
    }

    [Fact]
    public void Subtotal_ShouldCalculateBasedOnTotalOfItems()
    {
        // Arrange
        var quote = new Quote
        {
            Coupon = new Coupon { Code = "test_code", Discount = 10, IsDiscountFixed = true },
        };

        quote.Items = [
            new QuoteItem
            {
                Quote = quote,
                Quantity = 1,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 100 }
            },
            new QuoteItem
            {
                Quote = quote,
                Quantity = 2,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 200 }
            },
        ];

        // Assert
        Assert.Equal(470, quote.Subtotal);
    }

    [Fact]
    public void Subtotal_ShouldReturnZero_WhenThereAreNoItems()
    {
        // Arrange
        var quote = new Quote();

        // Assert
        Assert.Equal(0, quote.Subtotal);
    }

    [Fact]
    public void Total_ShouldBeTheSumOfSubtotalAndTax()
    {
        // Arrange
        var quote = new Quote
        {
            Coupon = new Coupon { Code = "test_code", Discount = 10, IsDiscountFixed = true },
            Tax = 10
        };

        quote.Items = [
            new QuoteItem
            {
                Quote = quote,
                Quantity = 1,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 100 }
            },
            new QuoteItem
            {
                Quote = quote,
                Quantity = 2,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 200 }
            },
        ];

        // Assert
        Assert.Equal(480, quote.Total);
    }

    [Fact]
    public void Total_ShouldBeEqualToSubtotalWhenThereIsNoTax()
    {
        // Arrange
        var quote = new Quote
        {
            Coupon = new Coupon { Code = "test_code", Discount = 10, IsDiscountFixed = true },
            Tax = 0
        };

        quote.Items = [
            new QuoteItem
            {
                Quote = quote,
                Quantity = 1,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 100 }
            },
            new QuoteItem
            {
                Quote = quote,
                Quantity = 2,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 200 }
            },
        ];

        // Assert
        Assert.Equal(470, quote.Total);
        Assert.Equal(quote.Subtotal, quote.Total);
    }

    [Fact]
    public void Total_ShouldBeZero_WhenThereAreNoItemsAndNoTax()
    {
        // Arrange
        var quote = new Quote
        {
            Items = [],
            Tax = 0
        };

        // Assert
        Assert.Equal(0, quote.Total);
    }

    [Fact]
    public void GetItemById_ShouldReturnTheItemWithTheGivenId_WhenItExists()
    {
        // Arrange
        var quote = new Quote
        {
            Items =
            [
                new QuoteItem { Id = 1 },
                new QuoteItem { Id = 2 },
            ]
        };

        // Act
        var result = quote.GetItemById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public void GetItemById_ShouldReturnNull_WhenTheItemDoesNotExist()
    {
        // Arrange
        var quote = new Quote
        {
            Items =
            [
                new QuoteItem { Id = 1 },
                new QuoteItem { Id = 2 },
            ]
        };

        // Act
        var result = quote.GetItemById(3);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetItemBy_ShouldReturnTheItemWithTheGivenProductId_WhenItExists()
    {
        // Arrange
        var quote = new Quote
        {
            Items =
            [
                new QuoteItem { ProductId = 1 },
                new QuoteItem { ProductId = 2 },
            ]
        };

        // Act
        var result = quote.GetItemBy(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ProductId);
    }

    [Fact]
    public void GetItemBy_ShouldReturnNull_WhenTheItemDoesNotExist()
    {
        // Arrange
        var quote = new Quote
        {
            Items =
            [
                new QuoteItem { ProductId = 1 },
                new QuoteItem { ProductId = 2 },
            ]
        };

        // Act
        var result = quote.GetItemBy(3);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Close_ShouldSetIsOpenToFalse()
    {
        // Arrange
        var quote = new Quote();

        // Act
        quote.Close();

        // Assert
        Assert.False(quote.IsOpen);
    }

    [Fact]
    public void IncludeItemsFrom_ShouldAddNewItems()
    {
        // Arrange
        var quote = new Quote();
        var otherQuote = new Quote();
        var newItem = new QuoteItem { Id = 1, ProductId = 1, Quantity = 2 };
        otherQuote.Items.Add(newItem);

        // Act
        quote.IncludeItemsFrom(otherQuote);

        // Assert
        Assert.Single(quote.Items);
        Assert.Equal(newItem.ProductId, quote.Items.First().ProductId);
        Assert.Equal(newItem.Quantity, quote.Items.First().Quantity);
    }

    [Fact]
    public void IncludeItemsFrom_ShouldCloneNewItems()
    {
        // Arrange
        var quote = new Quote();
        var otherQuote = new Quote();
        var newItem = new QuoteItem { Id = 1, ProductId = 1, Quantity = 2 };
        otherQuote.Items.Add(newItem);

        // Act
        quote.IncludeItemsFrom(otherQuote);

        // Assert
        Assert.Single(quote.Items);
        Assert.NotSame(newItem, quote.Items.First());
    }

    [Fact]
    public void IncludeItemsFrom_ShouldUpdateExistingItemsWithMatchingProductIds_IncresingTheirQuantity()
    {
        // Arrange
        var quote = new Quote();
        var existingItem = new QuoteItem { Id = 1, ProductId = 1, Quantity = 2 };
        quote.Items.Add(existingItem);

        var otherQuote = new Quote();
        var newItem = new QuoteItem { Id = 2, ProductId = 1, Quantity = 3 };
        otherQuote.Items.Add(newItem);

        // Act
        quote.IncludeItemsFrom(otherQuote);

        // Assert
        Assert.Single(quote.Items);
        Assert.Equal(existingItem.ProductId, quote.Items.First().ProductId);
        Assert.Equal(5, quote.Items.First().Quantity);
    }

    [Fact]
    public void IsFree_ShouldBeFalseWhenTotalIsNotZero()
    {
        // Arrange
        var quote = new Quote();

        quote.Items = [
            new QuoteItem
            {
                Quote = quote,
                Quantity = 1,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 100 }
            },
            new QuoteItem
            {
                Quote = quote,
                Quantity = 2,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 200 }
            },
        ];

        // Assert
        Assert.False(quote.IsFree());
    }

    [Fact]
    public void IsFree_ShouldBeTrueWhenTotalIsZero()
    {
        // Arrange
        var quote = new Quote();

        quote.Items = [
            new QuoteItem
            {
                Quote = quote,
                Quantity = 1,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 0 }
            },
            new QuoteItem
            {
                Quote = quote,
                Quantity = 2,
                Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 0 }
            },
        ];

        // Assert
        Assert.True(quote.IsFree());
    }

    [Fact]
    public void IsFromCustomer_ShouldReturnTrue_WhenCustomerIdIsSet()
    {
        // Arrange
        var quote = new Quote
        {
            CustomerId = 1
        };

        // Assert
        Assert.True(quote.IsFromCustomer);
    }

    [Fact]
    public void IsFromCustomer_ShouldReturnFalse_WhenCustomerIdIsNotSet()
    {
        // Arrange
        var quote = new Quote();

        // Assert
        Assert.False(quote.IsFromCustomer);
    }

    [Fact]
    public void GetDiscountForItem_ReturnsZero_WhenThereIsNoCoupon()
    {
        // Arrange
        var item = new QuoteItem
        {
            Quantity = 2,
            Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 100 }
        };

        var quote = new Quote { Items = [ item ] };

        // Act
        var discount = quote.GetDiscountForItem(item);

        // Assert
        Assert.Equal(0.0M, discount);
    }

    [Fact]
    public void GetDiscountForItem_ReturnsTheCorrectAmount_WhenTheCouponIsFixed()
    {
        // Arrange
        var item = new QuoteItem
        {
            Quantity = 2,
            Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 100 }
        };

        var quote = new Quote {
            Coupon = new Coupon { Code = "test_code", Discount = 10, IsDiscountFixed = true },
            Items = [ item ]
        };

        // Act
        var discount = quote.GetDiscountForItem(item);

        // Assert
        Assert.Equal(20, discount);
    }

    [Fact]
    public void GetDiscountForItem_ReturnsTheCorrectAmount_WhenTheCouponIsByPercentage()
    {
        // Arrange
        var item = new QuoteItem
        {
            Quantity = 1,
            Product = new Product { Sku = "test_sku", Name = "test_name", BasePrice = 200 }
        };

        var quote = new Quote {
            Coupon = new Coupon { Code = "test_code", Discount = 15, IsDiscountFixed = false },
            Items = [ item ]
        };

        // Act
        var discount = quote.GetDiscountForItem(item);

        // Assert
        Assert.Equal(30, discount);
    }
}
