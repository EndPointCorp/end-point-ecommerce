using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EndPointCommerce.RazorTemplates.Services;

public interface IRazorViewRenderer
{
    Task<string> Render<TModel>(string viewName, TModel model);
}

// Code from:
// - https://scottsauber.com/2018/07/07/walkthrough-creating-an-html-email-template-with-razor-and-razor-class-libraries-and-rendering-it-from-a-net-standard-class-library/
// - https://github.com/aspnet/Entropy/blob/master/samples/Mvc.RenderViewToString/RazorViewToStringRenderer.cs
// - https://stackoverflow.com/questions/63802400/return-view-as-string-in-net-core-3-0/64337478#64337478
public class RazorViewRenderer : IRazorViewRenderer
{
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly string _websiteUrl;
    private readonly string _websiteLogoUrl;

    public RazorViewRenderer(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceScopeFactory serviceScopeFactory,
        IConfiguration config
    ) {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceScopeFactory = serviceScopeFactory;
        _websiteUrl = config["WebsiteUrl"]!;
        _websiteLogoUrl = config["WebsiteLogoUrl"]!;
    }

    public async Task<string> Render<TModel>(string viewName, TModel model)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var httpContext = new DefaultHttpContext() { RequestServices = scope.ServiceProvider };
        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

        var view = FindView(actionContext, viewName);

        var viewData = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model,
            ["WebsiteUrl"] = _websiteUrl,
            ["WebsiteLogoUrl"] = _websiteLogoUrl
        };

        var tempData = new TempDataDictionary(httpContext, _tempDataProvider);

        using var output = new StringWriter();

        var viewContext = new ViewContext(
            actionContext,
            view,
            viewData,
            tempData,
            output,
            new HtmlHelperOptions()
        );

        await view.RenderAsync(viewContext);

        return output.ToString();
    }

    private IView FindView(ActionContext actionContext, string viewName)
    {
        var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
        if (getViewResult.Success)
        {
            return getViewResult.View;
        }

        var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
        if (findViewResult.Success)
        {
            return findViewResult.View;
        }

        var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
        var errorMessage = string.Join(
            Environment.NewLine,
            new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations)
        );

        throw new InvalidOperationException(errorMessage);
    }
}
