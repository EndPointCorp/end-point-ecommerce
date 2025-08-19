// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EndPointEcommerce.AdminPortal.Pages;

public class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;

    public PrivacyModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}

