using EndPointCommerce.Infrastructure.Configuration;
using EndPointCommerce.Infrastructure.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;

namespace EndPointCommerce.UnitTests.Infrastructure.Services
{
    public class MailerTests
    {
        private readonly Mock<ISmtpClient> _mockSmtpClient;
        private readonly Mock<IOptions<MailSettings>> _mockOptions;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ILogger<Mailer>> _mockLogger;
        private readonly Mailer _subject;

        public MailerTests()
        {
            _mockSmtpClient = new Mock<ISmtpClient>();

            _mockOptions = new Mock<IOptions<MailSettings>>();
            _mockOptions.Setup(m => m.Value).Returns(new MailSettings
            {
                Server = "smtp.test.com",
                Port = 587,
                SenderName = "Test Sender",
                SenderEmail = "sender@test.com",
                UserName = "test_username",
                Password = "test_password"
            });

            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["MailCcAddresses"]).Returns("cc1@test.com;cc2@test.com");
            _mockConfig.Setup(c => c["MailBccAddresses"]).Returns("bcc1@test.com;bcc2@test.com");

            _mockLogger = new Mock<ILogger<Mailer>>();

            _subject = new Mailer(
                _mockSmtpClient.Object,
                _mockOptions.Object,
                _mockConfig.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task SendMailAsync_ShouldReturnTrue_WhenMailIsSentSuccessfully()
        {
            // Arrange
            var mailData = new MailData
            {
                To = "recipient@test.com",
                ToName = "Test Recipient",
                Subject = "Test Subject",
                Body = "Test Body"
            };

            // Act
            var result = await _subject.SendMailAsync(mailData);

            // Assert
            Assert.True(result);

            _mockSmtpClient.Verify(s => s.ConnectAsync("smtp.test.com", 587, MailKit.Security.SecureSocketOptions.StartTls, default), Times.Once);
            _mockSmtpClient.Verify(s => s.AuthenticateAsync("test_username", "test_password", default), Times.Once);
            _mockSmtpClient.Verify(
                s => s.SendAsync(
                    It.Is<MimeMessage>(m =>
                        m.From.Count == 1 &&
                        m.From.Mailboxes.Any(mb => mb.Name == "Test Sender" && mb.Address == "sender@test.com") &&

                        m.To.Count == 1 &&
                        m.To.Mailboxes.Any(mb => mb.Name == "Test Recipient" && mb.Address == "recipient@test.com") &&

                        m.Cc.Count == 2 &&
                        m.Cc.Mailboxes.Any(mb => mb.Name == "cc1@test.com" && mb.Address == "cc1@test.com") &&
                        m.Cc.Mailboxes.Any(mb => mb.Name == "cc2@test.com" && mb.Address == "cc2@test.com") &&

                        m.Bcc.Count == 2 &&
                        m.Bcc.Mailboxes.Any(mb => mb.Name == "bcc1@test.com" && mb.Address == "bcc1@test.com") &&
                        m.Bcc.Mailboxes.Any(mb => mb.Name == "bcc2@test.com" && mb.Address == "bcc2@test.com") &&

                        m.Subject == "Test Subject" &&

                        m.TextBody == "Test Body" &&
                        m.HtmlBody == "Test Body"
                    ),
                    default,
                    null
                ),
                Times.Once
            );
            _mockSmtpClient.Verify(s => s.DisconnectAsync(true, default), Times.Once);
        }

        [Fact]
        public async Task SendMailAsync_ShouldReturnFalse_WhenAnExceptionIsThrownByTheSmtpClient()
        {
            // Arrange
            var mailData = new MailData
            {
                To = "recipient@test.com",
                ToName = "Test Recipient",
                Subject = "Test Subject",
                Body = "Test Body"
            };

            _mockSmtpClient
                .Setup(
                    s => s.ConnectAsync(
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.IsAny<MailKit.Security.SecureSocketOptions>(),
                        default
                    )
                )
                .ThrowsAsync(new Exception("test_exception"));

            // Act
            var result = await _subject.SendMailAsync(mailData);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SendMailAsync_ShouldLogTheError_WhenAnExceptionIsThrownByTheSmtpClient()
        {
            // Arrange
            var mailData = new MailData
            {
                To = "recipient@test.com",
                ToName = "Test Recipient",
                Subject = "Test Subject",
                Body = "Test Body"
            };

            _mockSmtpClient
                .Setup(
                    s => s.ConnectAsync(
                        It.IsAny<string>(),
                        It.IsAny<int>(),
                        It.IsAny<MailKit.Security.SecureSocketOptions>(),
                        default
                    )
                )
                .ThrowsAsync(new Exception("test_exception"));

            // Act
            await _subject.SendMailAsync(mailData);

            // Assert
            Assert.Single(_mockLogger.Invocations);
        }
    }
}