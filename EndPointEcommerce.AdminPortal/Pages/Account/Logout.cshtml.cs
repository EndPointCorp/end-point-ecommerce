// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointEcommerce.Domain.Interfaces;

namespace EndPointEcommerce.AdminPortal.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly IIdentityService _identityService;

        public LogoutModel(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<IActionResult> OnGetAsync(string? returnUrl)
        {
            await _identityService.LogoutAsync();
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}
