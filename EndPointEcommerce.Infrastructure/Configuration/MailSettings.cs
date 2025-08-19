// Copyright 2025 End Point Corporation. Apache License, version 2.0.

namespace EndPointEcommerce.Infrastructure.Configuration;

public class MailSettings
{
    public required string Server { get; set; }
    public required int Port { get; set; }
    public required string SenderName { get; set; }
    public required string SenderEmail { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
}