// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.Net;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Infrastructure.Services;
using EndPointEcommerce.RazorTemplates.Views;
using EndPointEcommerce.RazorTemplates.Services;
using EndPointEcommerce.RazorTemplates.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components;
using Moq;

namespace EndPointEcommerce.Tests.Infrastructure.Services
{
    public class IdentityEmailSenderTests
    {
        private readonly Mock<IMailer> _mockMailer;
        private readonly Mock<IRazorViewRenderer> _mockRazorViewRenderer;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly IdentityEmailSender _subject;

        public IdentityEmailSenderTests()
        {
            _mockMailer = new Mock<IMailer>();

            _mockRazorViewRenderer = new Mock<IRazorViewRenderer>();
            _mockRazorViewRenderer
                .Setup(m => m.Render<IComponent, IdentityEmailViewModel>(It.IsAny<IdentityEmailViewModel>()))
                .ReturnsAsync("test_rendered_body");

            _mockConfig = new Mock<IConfiguration>();
            _mockConfig
                .Setup(m => m["PasswordResetUrl"])
                .Returns("test_password_reset_url");

            _subject = new IdentityEmailSender(_mockMailer.Object, _mockRazorViewRenderer.Object, _mockConfig.Object);
        }

        private static User CreateUser()
        {
            return new User
            {
                Customer = new Customer
                {
                    Name = "test_name",
                    LastName = "test_last_name",
                    Email = "test_customer@email.com"
                }
            };
        }

        [Fact]
        public async Task SendConfirmationLinkAsync_CallsOnTheRazorViewRendererToRenderTheEmailBody_AndSendsTheEmail()
        {
            // Arrange
            User user = CreateUser();

            var email = "test@email.com";
            var confirmationLink = "test_confirmation_link";

            // Act
            await _subject.SendConfirmationLinkAsync(user, email, confirmationLink);

            // Assert
            _mockRazorViewRenderer.Verify(
                m => m.Render<AccountConfirmation, IdentityEmailViewModel>(
                    It.Is<IdentityEmailViewModel>(
                        vm =>
                            vm.User == user &&
                            vm.Link == confirmationLink
                    )
                ),
                Times.Once
            );

            _mockMailer.Verify(m => m.SendMailAsync(It.Is<MailData>(msg =>
                msg.To == email &&
                msg.ToName == user.Greeting &&
                msg.Subject == "Please confirm your email" &&
                msg.Body == "test_rendered_body"
            )), Times.Once);
        }


        [Fact]
        public async Task SendPasswordResetCodeAsync_CallsOnTheRazorViewRendererToRenderTheEmailBody_AndSendsTheEmail()
        {
            // Arrange
            User user = CreateUser();

            var email = "test@email.com";
            var resetCode = "test_reset_code";

            // Act
            await _subject.SendPasswordResetCodeAsync(user, email, resetCode);

            // Assert
            _mockRazorViewRenderer.Verify(
                m => m.Render<PasswordReset, IdentityEmailViewModel>(
                    It.Is<IdentityEmailViewModel>(
                        vm =>
                            vm.User == user &&
                            vm.Link == $"test_password_reset_url?email={WebUtility.UrlEncode(email)}&resetCode=test_reset_code"
                    )
                ),
                Times.Once
            );

            _mockMailer.Verify(m => m.SendMailAsync(It.Is<MailData>(msg =>
                msg.To == email &&
                msg.ToName == user.Greeting &&
                msg.Subject == "Reset your password" &&
                msg.Body == "test_rendered_body"
            )), Times.Once);
        }

        [Fact]
        public async Task SendPasswordResetLinkAsync_ThrowsNotImplementedException()
        {
            // Arrange
            User user = CreateUser();

            // Act & Assert
            await Assert.ThrowsAsync<NotImplementedException>(() => _subject.SendPasswordResetLinkAsync(user, "test@email.com", "test_reset_link"));
        }
    }
}