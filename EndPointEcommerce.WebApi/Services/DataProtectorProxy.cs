using Microsoft.AspNetCore.DataProtection;

namespace EndPointEcommerce.WebApi.Services;

public interface IDataProtectorProxy
{
    string Protect(string plaintext);
    string Unprotect(string protectedData);
}

public class DataProtectorProxy : IDataProtectorProxy
{
    private const string DATA_PROTECTOR_PURPOSE = "EndPointEcommerceCookieProtector";

    private readonly IDataProtector _protector;

    public DataProtectorProxy(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(DATA_PROTECTOR_PURPOSE);
    }

    public string Protect(string plaintext) => _protector.Protect(plaintext);
    public string Unprotect(string protectedData) => _protector.Unprotect(protectedData);
}
