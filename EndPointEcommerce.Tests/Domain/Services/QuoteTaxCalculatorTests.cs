// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using static EndPointEcommerce.Domain.Interfaces.ITaxCalculator;

namespace EndPointEcommerce.Tests.Domain.Services
{
    public class QuoteTaxCalculatorTests
    {
        private readonly Mock<IQuoteRepository> _mockQuoteRepository;
        private readonly Mock<ITaxCalculator> _mockTaxCalculator;
        private readonly Mock<ILogger<QuoteTaxCalculator>> _mockLogger;
        private readonly QuoteTaxCalculator _subject;

        public QuoteTaxCalculatorTests()
        {
            _mockQuoteRepository = new Mock<IQuoteRepository>();

            _mockTaxCalculator = new Mock<ITaxCalculator>();
            _mockTaxCalculator
                .Setup(m => m.Calculate(It.IsAny<Quote>()))
                .ReturnsAsync(new TaxResponse { AmountToCollect = 10.0M });

            _mockLogger = new Mock<ILogger<QuoteTaxCalculator>>();

            _subject = new QuoteTaxCalculator(
                _mockQuoteRepository.Object,
                _mockTaxCalculator.Object,
                _mockLogger.Object
            );
        }

        private static Quote CreateQuote()
        {
            var quote = new Quote
            {
                ShippingAddress = new Address
                {
                    Name = "test_name",
                    LastName = "test_last_name",
                    Street = "123 Main St",
                    City = "Anytown",
                    ZipCode = "12345",
                    CountryId = 1,
                    StateId = 1
                }
            };

            quote.Items =
            [
                new QuoteItem
                {
                    Quote = quote,
                    Quantity = 1,
                    Product = new Product {
                        Sku = "test_sku",
                        Name = "test_name",
                        BasePrice = 100
                    }
                }
            ];

            return quote;
        }

        [Fact]
        public async Task Run_ShouldNotCalculateTax_WhenTheQuoteHasNoItems()
        {
            // Arrange
            var quote = CreateQuote();
            quote.Items.Clear();

            // Act
            var result = await _subject.Run(quote);

            // Assert
            Assert.Equal(0.0M, result.Tax);
            _mockQuoteRepository.Verify(q => q.UpdateAsync(quote), Times.Once);
            _mockTaxCalculator.Verify(t => t.Calculate(It.IsAny<Quote>()), Times.Never);
        }

        [Fact]
        public async Task Run_ShouldNotCalculateTax_WhenTheQuoteHasNoCost()
        {
            // Arrange
            var quote = CreateQuote();
            quote.Items.First().Product.BasePrice = 0;

            // Act
            var result = await _subject.Run(quote);

            // Assert
            Assert.Equal(0.0M, result.Tax);
            _mockQuoteRepository.Verify(q => q.UpdateAsync(quote), Times.Once);
            _mockTaxCalculator.Verify(t => t.Calculate(It.IsAny<Quote>()), Times.Never);
        }

        [Fact]
        public async Task Run_ShouldNotCalculateTax_WhenTheQuoteHasNoShippingAddress()
        {
            // Arrange
            var quote = CreateQuote();
            quote.ShippingAddress = null;

            // Act
            var result = await _subject.Run(quote);

            // Assert
            Assert.Equal(0.0M, result.Tax);
            _mockQuoteRepository.Verify(m => m.UpdateAsync(quote), Times.Once);
            _mockTaxCalculator.Verify(m => m.Calculate(It.IsAny<Quote>()), Times.Never);
        }

        [Fact]
        public async Task Run_ShouldReturnTheQuoteWithTheCalculatedTax()
        {
            // Arrange
            var quote = CreateQuote();

            // Act
            var result = await _subject.Run(quote);

            // Assert
            Assert.Equal(10.0M, result.Tax);
        }

        [Fact]
        public async Task Run_ShouldReturnTheQuoteWithoutTheCalculatedTax_WhenTheTaxCalculatorReturnsNull()
        {
            // Arrange
            var quote = CreateQuote();

            _mockTaxCalculator
                .Setup(m => m.Calculate(It.IsAny<Quote>()))
                .ReturnsAsync((TaxResponse)null!);

            // Act
            var result = await _subject.Run(quote);

            // Assert
            Assert.Equal(0.0M, result.Tax);
        }

        [Fact]
        public async Task Run_ShouldCallOnTheTaxCalculatorToCalculateTax()
        {
            // Arrange
            var quote = CreateQuote();

            // Act
            var result = await _subject.Run(quote);

            // Assert
            _mockTaxCalculator.Verify(m => m.Calculate(quote), Times.Once);
        }

        [Fact]
        public async Task Run_ShouldCallOnTheRepositoryToUpdateTheTaxOnTheQuote()
        {
            // Arrange
            var quote = CreateQuote();

            // Act
            var result = await _subject.Run(quote);

            // Assert
            _mockQuoteRepository.Verify(m => m.UpdateAsync(quote), Times.Once);
        }

        [Fact]
        public async Task Run_ShouldLogAnError_WhenAnExceptionIsThrownByTheTaxCalculator()
        {
            // Arrange
            var quote = CreateQuote();
            quote.Id = 1;

            _mockTaxCalculator
                .Setup(t => t.Calculate(It.IsAny<Quote>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _subject.Run(quote);

            // Assert
            Assert.Single(_mockLogger.Invocations);
        }
    }
}