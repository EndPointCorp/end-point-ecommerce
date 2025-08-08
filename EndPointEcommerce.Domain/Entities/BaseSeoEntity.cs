using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.Domain.Entities;

/// <summary>
/// Generic entity class with properties related to search engine optimization.
/// </summary>
public abstract class BaseSeoEntity : BaseAuditEntity
{
    [Display(Name = "URL Key")]
    [RegularExpression(@"^[a-zA-Z0-9._~-]+$", ErrorMessage = "Only URI-safe characters are allowed.")]
    public virtual string? UrlKey { get; set; }

    [Display(Name = "Meta Title")]
    public string? MetaTitle { get; set; }

    [Display(Name = "Meta Keywords")]
    public string? MetaKeywords { get; set; }

    [Display(Name = "Meta Description")]
    public string? MetaDescription { get; set; }
}
