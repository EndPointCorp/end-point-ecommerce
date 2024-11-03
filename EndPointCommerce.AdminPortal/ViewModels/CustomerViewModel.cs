using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.AdminPortal.ViewModels;

/// <summary>
/// View model for customers.
/// </summary>
public class CustomerViewModel : Customer
{
    public Customer ToModel()
    {
        return new Customer()
        {
            Id = Id,
            Name = Name,
            LastName = LastName,
            Email = Email,
        };
    }

    public static CustomerViewModel CreateDefault()
    {
        var customerViewModel = new CustomerViewModel()
        {
            Name = "",
            LastName = "",
            Email = "",
        };
        return customerViewModel;
    }

    public static CustomerViewModel FromModel(Customer model)
    {
        var customerViewModel = CreateDefault();
        customerViewModel.Id = model.Id;
        customerViewModel.Name = model.Name;
        customerViewModel.LastName = model.LastName;
        customerViewModel.Email = model.Email;
        return customerViewModel;
    }
}