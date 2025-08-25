// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using System.ComponentModel.DataAnnotations;

namespace EndPointEcommerce.WebApi.ResourceModels;

public class UserPut
{
    [Required]
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;

    public Domain.Entities.User UpdateEntity(Domain.Entities.User entity)
    {
        entity.Email = Email;
        entity.PhoneNumber = PhoneNumber;
        if (entity.Customer != null)
        {
            entity.Customer.Email = Email;
            entity.Customer.Name = Name;
            entity.Customer.LastName = LastName;
        }
        return entity;
    }
}
