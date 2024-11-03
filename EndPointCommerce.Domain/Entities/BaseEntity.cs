namespace EndPointCommerce.Domain.Entities;

/// <summary>
/// Base entity class.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }

    public bool Equals(BaseEntity that) => Id == that.Id;
}