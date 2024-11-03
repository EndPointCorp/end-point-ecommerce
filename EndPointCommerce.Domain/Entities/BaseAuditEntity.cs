using System.ComponentModel.DataAnnotations;

namespace EndPointCommerce.Domain.Entities;

/// <summary>
/// Generic entity class with audit properties.
/// </summary>
public abstract class BaseAuditEntity: BaseEntity
{
    /// <summary>
    /// Gets or sets the user reference that created the entity.
    /// </summary>
    [Display(Name = "Created By")]
    public int? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the user reference that last modified the entity.
    /// </summary>
    [Display(Name = "Modified By")]
    public int? ModifiedBy { get; set; }

    /// <summary>
    /// Gets or sets the user reference that deleted the entity.
    /// </summary>
    public int? DeletedBy { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DateTime"/> an entity was first created.
    /// </summary>
    [Display(Name = "Date Created")]
    public DateTime? DateCreated { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DateTime"/> an entity was last modified.
    /// </summary>
    [Display(Name = "Date Modified")]
    public DateTime? DateModified { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DateTime"/> an entity was deleted or restored.
    /// </summary>
    [Display(Name = "Date Deleted/Restored")]
    public DateTime? DateDeleted { get; set; }

    /// <summary>
    /// Gets or sets whether an entity is deleted.
    /// </summary>
    [Display(Name = "Is Deleted")]
    public bool Deleted { get; set; } = false;
}