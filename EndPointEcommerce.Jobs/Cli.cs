using CommandLine;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EndPointEcommerce.Jobs;

[Verb("create_admin_user", HelpText = "Creates a user account.")]
class CreateAdminUserOptions
{
    [Option('u', "username", Required = true, HelpText = "The username of the user.")]
    public string UserName { get; set; } = string.Empty;

    [Option('e', "email", Required = true, HelpText = "The email of the user.")]
    public string Email { get; set; } = string.Empty;

    [Option('p', "password", Required = true, HelpText = "The password of the user.")]
    public string Password { get; set; } = string.Empty;
}

static class Cli
{
    public static async Task<int> Run(string[] args, IHost host) =>
        await Parser.Default.ParseArguments<
            CreateAdminUserOptions
        >(args)
            .MapResult(
                (CreateAdminUserOptions _o) => RunCreateAdminUser(_o, host),
                _ => Task.FromResult(1)
            );

    static async Task<int> RunCreateAdminUser(CreateAdminUserOptions options, IHost host)
    {
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var identityService = host.Services.GetRequiredService<IIdentityService>();

        logger.LogInformation("Creating user with {UserName}", options.UserName);

        var result = await identityService.AddAsync(
            new User { UserName = options.UserName, Email = options.Email },
            options.Password,
            User.ADMIN_ROLE
        );

        if (!result.Succeeded)
        {
            logger.LogError(string.Join(" ", result.Errors.Select(x => x.Description)));
            return 1;
        }

        logger.LogInformation("User {UserName} created successfully", options.UserName);
        return 0;
    }
}
