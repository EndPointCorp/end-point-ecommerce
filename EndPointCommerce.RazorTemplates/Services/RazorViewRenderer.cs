using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace EndPointCommerce.RazorTemplates.Services;

public interface IRazorViewRenderer
{
    Task<string> Render<TView, TViewModel>(TViewModel model) where TView : IComponent;
}

public class RazorViewRenderer : IRazorViewRenderer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;

    public RazorViewRenderer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    public async Task<string> Render<TView, TViewModel>(TViewModel model) where TView : IComponent
    {
        await using var htmlRenderer = new HtmlRenderer(_serviceProvider, _loggerFactory);

        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            var output = await htmlRenderer.RenderComponentAsync<TView>(
                ParameterView.FromDictionary(
                    new Dictionary<string, object?> { { "Model", model } }
                )
            );

            return output.ToHtmlString();
        });

        return html;
    }
}