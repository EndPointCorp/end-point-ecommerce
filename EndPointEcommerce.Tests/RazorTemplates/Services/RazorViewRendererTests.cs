using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.RazorTemplates;
using EndPointEcommerce.RazorTemplates.Services;
using EndPointEcommerce.RazorTemplates.ViewModels;
using EndPointEcommerce.RazorTemplates.Views;
using EndPointEcommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace EndPointEcommerce.Tests.RazorTemplates.Services
{
    public class RazorViewRendererTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly IRazorViewRenderer _subject;

        public RazorViewRendererTests(WebApplicationFactory<Program> factory)
        {
            _subject = factory.Services.GetRequiredService<IRazorViewRenderer>();
        }

        [Fact]
        public async Task Render_CanRenderTheAccountConfirmationEmailTemplate()
        {
            // Act
            var result = await _subject.Render<AccountConfirmation, IdentityEmailViewModel>(
                new IdentityEmailViewModel()
                {
                    User = new User {
                        Customer = new()
                        {
                            Name = "test_name",
                            LastName = "test_last_name",
                            Email = "test@email.com"
                        }
                    },

                    Link = "test_confirmation_link"
                }
            );

            // Assert
            Assert.Contains("<!DOCTYPE html", result);
            Assert.Contains("Thank you for signing up with EndPointEcommerce.com", result);
            Assert.Contains("test_name test_last_name", result);
            Assert.Contains("Please <a href=\"test_confirmation_link\"", result);
            Assert.Contains("click here</a> to confirm your account.", result);
        }

        [Fact]
        public async Task Render_CanRenderThePasswordResetEmailTemplate()
        {
            // Act
            var result = await _subject.Render<PasswordReset, IdentityEmailViewModel>(
                new IdentityEmailViewModel()
                {
                    User = new User {
                        Customer = new()
                        {
                            Name = "test_name",
                            LastName = "test_last_name",
                            Email = "test@email.com"
                        }
                    },

                    Link = "test_password_reset_link"
                }
            );

            // Assert
            Assert.Contains("<!DOCTYPE html", result);
            Assert.Contains("test_name test_last_name", result);
            Assert.Contains("<a href=\"test_password_reset_link\"", result);
            Assert.Contains("If you did not request a password reset, please ignore this email.", result);
        }

        [Fact]
        public async Task Render_CanRenderTheOrderConfirmationEmailTemplate()
        {
            // Act
            var orderGuid = Guid.NewGuid();

            var result = await _subject.Render<OrderConfirmation, OrderConfirmationViewModel>(
                new OrderConfirmationViewModel()
                {
                    Order = new Order
                    {
                        Id = 123,
                        OrderGuid = orderGuid,
                        DateCreated = new DateTime(2010, 10, 10),
                        Customer = new Customer
                        {
                            Name = "test_name",
                            LastName = "test_last_name",
                            Email = "test_customer@email.com"
                        },
                        Items = [],
                        ShippingAddress = new()
                        {
                            Name = "test_name",
                            LastName = "test_last_name",
                            ZipCode = "12345",
                            CountryId = 1,
                            Country = new Country() { Name = "United States", Code = "US" },
                            StateId = 1,
                            State = new State() { Name = "New York", Abbreviation = "NY" },
                            City = "MyCity",
                            Street = "123 Main St",
                            StreetTwo = "Unit A"
                        },
                        BillingAddress = new()
                        {
                            Name = "test_name",
                            LastName = "test_last_name",
                            ZipCode = "12345",
                            CountryId = 1,
                            Country = new Country() { Name = "United States", Code = "US" },
                            StateId = 1,
                            State = new State() { Name = "New York", Abbreviation = "NY" },
                            City = "MyCity",
                            Street = "123 Main St",
                            StreetTwo = "Unit A"
                        },
                        Status = new OrderStatus { Name = OrderStatus.PENDING },
                        PaymentMethod = new PaymentMethod { Name = PaymentMethod.CREDIT_CARD }
                    },
                    OrderDetailsUrl = "test_order_details_url",
                    ProductImagesUrl = "test_product_images_url"
                }
            );

            // Assert
            Assert.Contains("<!DOCTYPE html", result);
            Assert.Contains("Thank you for your order!", result);
            Assert.Contains($"<a href=\"test_order_details_url/{orderGuid}\"", result);
            Assert.Contains($"Sunday, October 10, 2010", result);
            Assert.Contains($"<a href=\"test_order_details_url/{orderGuid}\"", result);
            Assert.Contains($"123", result);
        }
    }
}
