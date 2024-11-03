using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Validation;

namespace EndPointCommerce.UnitTests.Domain.Validation
{
    public class QuoteValidatorTest
    {
        private static Quote CreateValidQuote() =>
            new()
            {
                IsOpen = true,
                CustomerId = 1,
                Email = "customer@example.com",

                ShippingAddress = new Address
                {
                    Name = "test_name",
                    LastName = "test_last_name",
                    Street = "123 Main St",
                    City = "Anytown",
                    ZipCode = "12345",
                    State = new State
                    {
                        Name = "New York",
                        Abbreviation = "NY"
                    },
                    StateId = 1
                },
                BillingAddress = new Address
                {
                    Name = "test_name",
                    LastName = "test_last_name",
                    Street = "123 Main St",
                    City = "Anytown",
                    ZipCode = "12345",
                    State = new State
                    {
                        Name = "New York",
                        Abbreviation = "NY"
                    },
                    StateId = 1
                },

                Items =
                    [
                        new QuoteItem
                        {
                            Id = 1,
                            QuoteId = 1,
                            ProductId = 1,
                            Quantity = 1
                        }
                    ]
            };

        [Fact]
        public void Validate_Succeeds_WhenTheQuoteIsValid_AndBelongsToACustomer()
        {
            // Arrange
            var quote = CreateValidQuote();
            quote.CustomerId = 1;

            // Act
            var (isValid, results) = QuoteValidator.Validate(quote);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Validate_Succeeds_WhenTheQuoteIsValid_AndBelongsToAGuest()
        {
            // Arrange
            var quote = CreateValidQuote();
            quote.CustomerId = null;

            // Act
            var (isValid, results) = QuoteValidator.Validate(quote);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Validate_Fails_WhenTheQuoteIsNotOpen()
        {
            // Arrange
            var quote = CreateValidQuote();
            quote.IsOpen = false;

            // Act
            var (isValid, results) = QuoteValidator.Validate(quote);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage == "The quote must be open.");
        }

        [Fact]
        public void Validate_Fails_WhenTheEmailIsInvalid()
        {
            // Arrange
            var quote = CreateValidQuote();
            quote.Email = "not_an_email";

            // Act
            var (isValid, results) = QuoteValidator.Validate(quote);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage == "The Email field is not a valid e-mail address.");
        }

        [Fact]
        public void Validate_Fails_WhenTheQuoteBelongsToAGuest_AndDoesNotHaveAnEmail()
        {
            // Arrange
            var quote = CreateValidQuote();
            quote.CustomerId = null;
            quote.Email = null;

            // Act
            var (isValid, results) = QuoteValidator.Validate(quote);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage == "An email is required for guest orders.");
        }

        [Fact]
        public void Validate_Fails_WhenTheQuoteHasNoShippingAddress()
        {
            // Arrange
            var quote = CreateValidQuote();
            quote.ShippingAddress = null;

            // Act
            var (isValid, results) = QuoteValidator.Validate(quote);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage == "The ShippingAddress field is required.");
        }

        [Fact]
        public void Validate_Fails_WhenTheQuoteHasNoBillingAddress()
        {
            // Arrange
            var quote = CreateValidQuote();
            quote.BillingAddress = null;

            // Act
            var (isValid, results) = QuoteValidator.Validate(quote);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage == "The BillingAddress field is required.");
        }

        [Fact]
        public void Validate_Fails_WhenTheQuoteHasNoItems()
        {
            // Arrange
            var quote = CreateValidQuote();
            quote.Items = [];

            // Act
            var (isValid, results) = QuoteValidator.Validate(quote);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage == "The quote must have at least one item.");
        }


        [Fact]
        public void Validate_Fails_WhenAnItemHasZeroQuantity()
        {
            // Arrange
            var quote = CreateValidQuote();
            quote.Items.First().Quantity = 0;

            // Act
            var (isValid, results) = QuoteValidator.Validate(quote);

            // Assert
            Assert.False(isValid);
            Assert.Single(results);
            Assert.Contains(results.Single().ErrorMessage!, "The field Quantity must be between 1 and 2147483647.");
            Assert.Equal(results.Single().MemberNames, ["Items", "1", "Quantity"]);
        }
    }
}
