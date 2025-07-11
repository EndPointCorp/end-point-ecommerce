using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using EndPointCommerce.WebApi.Services;

namespace EndPointCommerce.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender<User> _identityEmailSender;
        private readonly string _confirmEmailUrl;

        public UserController(
            IIdentityService identityService,
            UserManager<User> userManager,
            IEmailSender<User> identityEmailSender,
            IConfiguration config
        ) {
            _identityService = identityService;
            _userManager = userManager;
            _identityEmailSender = identityEmailSender;
            _confirmEmailUrl = config["ConfirmEmailUrl"]!;
        }

        // GET: api/User
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ResourceModels.User>> GetUser()
        {
            var user = await _identityService.FindByUserNameAsync(User.Identity!.Name!);
            if (user == null || !user.IsCustomer)
            {
                return NotFound();
            }

            return ResourceModels.User.FromEntity(user);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<ResourceModels.User>> PostUser([FromBody] ResourceModels.UserPost user)
        {
            var userEntity = user.ToEntity();

            var result = await _identityService.AddAsync(userEntity, user.Password, Domain.Entities.User.CUSTOMER_ROLE);
            if (!result.Succeeded) return BadRequest(result.ToHttpValidationProblemDetails());

            var code = await _identityService.GenerateEmailConfirmationCodeAsync(userEntity);
            var emailLink = $"{_confirmEmailUrl}?userId={userEntity.Id}&code={code}";
            await _identityEmailSender.SendConfirmationLinkAsync(userEntity, user.Email, emailLink);

            return ResourceModels.User.FromEntity(userEntity);
        }

        // POST: api/User/Logout
        [HttpPost("Logout")]
        public async Task<ActionResult<ResourceModels.User>> PostLogout()
        {
            await _identityService.LogoutAsync();

            return Ok();
        }

        // PUT: api/User
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<ResourceModels.User>> PutUser([FromBody] ResourceModels.UserPut user)
        {
            var userEntity = await _identityService.FindByUserNameAsync(User.Identity!.Name!);
            if (userEntity == null || !userEntity.IsCustomer)
            {
                return NotFound();
            }

            userEntity = user.UpdateEntity(userEntity);

            // Set the new password if requested
            var newPassword = string.Empty;
            if ((!string.IsNullOrEmpty(user.CurrentPassword)) && (!string.IsNullOrEmpty(user.NewPassword)))
            {
                if (!await _identityService.IsPasswordValid(userEntity, user.CurrentPassword))
                    return BadRequest("Current password is incorrect.");
                newPassword = user.NewPassword;
            }

            // Update the user
            var result = await _identityService.UpdateAsync(userEntity, newPassword, Domain.Entities.User.CUSTOMER_ROLE);
            if (!result.Succeeded)
            {
                return BadRequest(string.Join(" ", result.Errors.ToList().Select(x => x.Description).ToArray()));
            }

            return ResourceModels.User.FromEntity(userEntity);
        }
    }
}
