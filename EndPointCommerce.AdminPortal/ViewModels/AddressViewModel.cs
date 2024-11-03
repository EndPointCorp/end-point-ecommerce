using EndPointCommerce.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using EndPointCommerce.Domain.Entities;

namespace EndPointCommerce.AdminPortal.ViewModels;

/// <summary>
/// View model for addresses.
/// </summary>
public class AddressViewModel : Address
{
    [Required]
    [Display(Name = "Customer")]
    public new int? CustomerId { get; set; }

    public IEnumerable<SelectListItem>? States { get; set; }
    public IEnumerable<SelectListItem>? Customers { get; set; }

    public Address ToModel()
    {
        return new Address()
        {
            Id = Id,
            Name = Name,
            LastName = LastName,
            Street = Street,
            StreetTwo = StreetTwo,
            City = City,
            ZipCode = ZipCode,
            PhoneNumber = PhoneNumber,
            CustomerId = CustomerId,
            StateId = StateId
        };
    }

    public async Task FillStates(IStateRepository stateRepository)
    {
        var states = await stateRepository.FetchAllAsync();
        States = states
            .Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            })
            .ToList();
    }

    public async Task FillCustomers(ICustomerRepository customerRepository)
    {
        var customers = await customerRepository.FetchAllAsync();
        Customers = customers
            .Select(x => new SelectListItem()
            {
                Text = x.FullName,
                Value = x.Id.ToString()
            })
            .ToList();
    }

    public static async Task<AddressViewModel> CreateDefault(int? customerId, IStateRepository stateRepository, ICustomerRepository customerRepository)
    {
        var addressViewModel = new AddressViewModel() {
            Name = "",
            LastName = "",
            City = "",
            CustomerId = customerId,
            StateId = (await stateRepository.FetchAllAsync()).First().Id,
            Street = "",
            ZipCode = ""
        };
        await addressViewModel.FillStates(stateRepository);
        await addressViewModel.FillCustomers(customerRepository);
        return addressViewModel;
    }

    public static async Task<AddressViewModel> FromModel(Address model, IStateRepository stateRepository, ICustomerRepository customerRepository)
    {
        var addressViewModel = await CreateDefault(model.CustomerId, stateRepository, customerRepository);
        addressViewModel.Id = model.Id;
        addressViewModel.Name = model.Name;
        addressViewModel.LastName = model.LastName;
        addressViewModel.Street = model.Street;
        addressViewModel.StreetTwo = model.StreetTwo;
        addressViewModel.City = model.City;
        addressViewModel.ZipCode = model.ZipCode;
        addressViewModel.PhoneNumber = model.PhoneNumber;
        addressViewModel.CustomerId = model.CustomerId;
        addressViewModel.StateId = model.StateId;
        return addressViewModel;
    }
}