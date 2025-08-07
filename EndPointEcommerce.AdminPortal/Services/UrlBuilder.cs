using Microsoft.AspNetCore.Mvc;

namespace EndPointEcommerce.AdminPortal.Services;

public interface IUrlBuilder
{
    string? Build(string? pageName, object? values);
}

public class UrlBuilder : IUrlBuilder
{
    private readonly IUrlHelper _urlHelper;

    public UrlBuilder(IUrlHelper urlHelper)
    {
        _urlHelper = urlHelper;
    }

    public string? Build(string? pageName, object? values) =>
        _urlHelper.Page(pageName, values);
}
