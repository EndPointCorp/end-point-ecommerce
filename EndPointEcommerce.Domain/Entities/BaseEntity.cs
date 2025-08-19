// Copyright 2025 End Point Corporation. Apache License, version 2.0.

namespace EndPointEcommerce.Domain.Entities;

/// <summary>
/// Base entity class.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }

    public bool Equals(BaseEntity that) => Id == that.Id;
}