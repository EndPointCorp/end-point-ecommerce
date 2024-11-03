using EndPointCommerce.Domain.Entities;
using EndPointCommerce.RazorTemplates;
using EndPointCommerce.RazorTemplates.Services;
using EndPointCommerce.RazorTemplates.ViewModels;
using EndPointCommerce.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace EndPointCommerce.IntegrationTests.RazorTemplates.Services
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
            var result = await _subject.Render(
                Templates.AccountConfirmation,
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
            Assert.Contains("<!DOCTYPE html>", result);
            Assert.Contains("Welcome to EndPointCommerce.com", result);
            Assert.Contains("test_name test_last_name", result);
            Assert.Contains("Please confirm your account by <a href=\"test_confirmation_link\">clicking here</a>", result);
            Assert.Contains("Thank you again, <strong>End Point Commerce</strong>", result);
        }

        [Fact]
        public async Task Render_CanRenderThePasswordResetEmailTemplate()
        {
            // Act
            var result = await _subject.Render(
                Templates.PasswordReset,
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
            Assert.Contains("<!DOCTYPE html>", result);
            Assert.Contains("test_name test_last_name", result);
            Assert.Contains("There was recently a request to change the password for your account.", result);
            Assert.Contains("<a href=\"test_password_reset_link\" target=\"_blank\">", result);
            Assert.Contains("Thank you again, <strong>End Point Commerce</strong>", result);
        }

        [Fact]
        public async Task Render_CanRenderTheOrderConfirmationEmailTemplate()
        {
            // Act
            var result = await _subject.Render(
                Templates.OrderConfirmation,
                new OrderConfirmationViewModel()
                {
                    Order = new Order
                    {
                        Id = 123,
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
                            StateId = 1,
                            State = new State() { Name = "New York", Abbreviation = "NY" },
                            City = "MyCity",
                            Street = "123 Main St",
                            StreetTwo = "Unit A"
                        },
                        PaymentMethod = new PaymentMethod { Name = PaymentMethod.CREDIT_CARD }
                    },

                    WebsiteShippingInfoUrl = "test_website_shipping_info_url"
                }
            );

            // Assert
            Assert.Contains("<!DOCTYPE html>", result);
            Assert.Contains("Thank You test_name test_last_name", result);
            Assert.Contains("Your Order #123 was placed on Sunday, October 10, 2010", result);
            Assert.Contains("<a href=\"test_website_shipping_info_url\">", result);
            Assert.Contains("Thank you again, <strong>End Point Commerce</strong>", result);
        }
    }
}
