using EndPointEcommerce.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using EndPointEcommerce.RazorTemplates.Services;
using EndPointEcommerce.RazorTemplates.ViewModels;
using EndPointEcommerce.RazorTemplates.Views;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace EndPointEcommerce.Infrastructure.Services;

public class IdentityEmailSender : IEmailSender<User>
{
    private readonly IMailer _mailer;
    private readonly IRazorViewRenderer _razorViewRenderer;
    private readonly string _passwordResetUrl;

    public IdentityEmailSender(IMailer mailer, IRazorViewRenderer razorViewRenderer, IConfiguration config)
    {
        _mailer = mailer;
        _razorViewRenderer = razorViewRenderer;
        _passwordResetUrl = config["PasswordResetUrl"]!;
    }

    public async Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        string body = await _razorViewRenderer.Render<AccountConfirmation, IdentityEmailViewModel>(
            new IdentityEmailViewModel()
            {
                User = user,
                Link = WebUtility.HtmlDecode(confirmationLink)
            }
        );

        await _mailer.SendMailAsync(new() {
            To = email,
            ToName = user.Greeting,
            Subject = "Please confirm your email",
            Body = body
        });
    }

    public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        string body = await _razorViewRenderer.Render<PasswordReset, IdentityEmailViewModel>(
            new IdentityEmailViewModel()
            {
                User = user,
                Link = $"{_passwordResetUrl}?email={WebUtility.UrlEncode(email)}&resetCode={resetCode}"
            }
        );

        await _mailer.SendMailAsync(new() {
            To = email,
            ToName = user.Greeting,
            Subject = "Reset your password",
            Body = body
        });
    }

    // This is not used for Web API projects.
    // https://github.com/dotnet/aspnetcore/issues/51278
    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        throw new NotImplementedException();
    }
}