using System.Net;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Infrastructure.Services;
using EndPointCommerce.RazorTemplates;
using EndPointCommerce.RazorTemplates.Services;
using EndPointCommerce.RazorTemplates.ViewModels;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EndPointCommerce.Tests.Infrastructure.Services
{
    public class OrderConfirmationMailerTests
    {
        private readonly Mock<IMailer> _mockMailer;
        private readonly Mock<IRazorViewRenderer> _mockRazorViewRenderer;
        private readonly OrderConfirmationMailer _subject;

        public OrderConfirmationMailerTests()
        {
            _mockMailer = new Mock<IMailer>();

            _mockRazorViewRenderer = new Mock<IRazorViewRenderer>();
            _mockRazorViewRenderer
                .Setup(m => m.Render(It.IsAny<string>(), It.IsAny<OrderConfirmationViewModel>()))
                .ReturnsAsync("test_rendered_body");

            _subject = new OrderConfirmationMailer(_mockMailer.Object, _mockRazorViewRenderer.Object);
        }

        private static Order CreateOrder()
        {
            return new Order
            {
                Id = 1,
                Customer = new Customer
                {
                    Name = "test_name",
                    LastName = "test_last_name",
                    Email = "test_customer@email.com"
                }
            };
        }

        [Fact]
        public async Task SendAsync_CallsOnTheRazorViewRendererToRenderTheEmailBody_AndSendsTheEmail()
        {
            // Arrange
            Order order = CreateOrder();

            // Act
            await _subject.SendAsync(order);

            // Assert
            _mockRazorViewRenderer.Verify(m => m.Render(Templates.OrderConfirmation, It.Is<OrderConfirmationViewModel>(vm =>
                vm.Order == order
            )), Times.Once);

            _mockMailer.Verify(m => m.SendMailAsync(It.Is<MailData>(msg =>
                msg.To == order.Customer.Email &&
                msg.ToName == order.Customer.FullName &&
                msg.Subject == "End Point Commerce: New Order # 1" &&
                msg.Body == "test_rendered_body"
            )), Times.Once);
        }
    }
}