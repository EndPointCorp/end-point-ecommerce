using Microsoft.AspNetCore.Mvc.RazorPages;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EndPointCommerce.AdminPortal.Pages.Countries
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ICountryRepository _repository;

        public IndexModel(ICountryRepository repository)
        {
            _repository = repository;
        }

        public IList<Country> Countries { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Countries = await _repository.FetchAllAsync();
        }

        public async Task<ActionResult> OnPostUpdateCountryAsync(int countryId, bool isEnabled)
        {
            var country = await _repository.FindByIdAsync(countryId);
            if (country == null) return new JsonResult(new { success = false, message = "Country not found" });

            country.IsEnabled = isEnabled;
            await _repository.UpdateAsync(country);

            return new JsonResult(new { success = true });
        }
    }
}
