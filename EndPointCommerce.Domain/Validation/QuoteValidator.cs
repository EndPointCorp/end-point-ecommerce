using System.ComponentModel.DataAnnotations;
using EndPointCommerce.Domain.Entities;
using FoolProof.Core;

namespace EndPointCommerce.Domain.Validation;

public class QuoteValidator
{
    class ValidatableQuote
    {
        [Range(typeof(bool), "true", "true", ErrorMessage = "The quote must be open.")]
        public bool IsOpen { get; set; }

        public bool IsFromCustomer { get; set; }

        [EmailAddress]
        [RequiredIfFalse("IsFromCustomer", ErrorMessage = "An email is required for guest orders.")]
        public string? Email { get; set; }

        [Required]
        public Address? ShippingAddress { get; set; }

        [Required]
        public Address? BillingAddress { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "The quote must have at least one item.")]
        public IList<ValidatableQuoteItem> Items { get; set; } = [];

        public static ValidatableQuote Build(Quote quote) =>
            new()
            {
                IsOpen = quote.IsOpen,
                IsFromCustomer = quote.IsFromCustomer,
                Email = quote.Email,
                ShippingAddress = quote.ShippingAddress,
                BillingAddress = quote.BillingAddress,
                Items = quote.Items
                    .Select(ValidatableQuoteItem.Build)
                    .ToList()
            };
    }

    class ValidatableQuoteItem
    {
        public int Id { get; set; }

        [Required]
        public int QuoteId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public static ValidatableQuoteItem Build(QuoteItem item) =>
            new()
            {
                Id = item.Id,
                QuoteId = item.QuoteId,
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };
    }

    public static (bool, IEnumerable<ValidationResult>) Validate(Quote quote)
    {
        var validatable = ValidatableQuote.Build(quote);
        var results = new List<ValidationResult>();

        var isValid = Validate(validatable, results);

        foreach (var item in validatable.Items)
        {
            var itemResults = new List<ValidationResult>();
            isValid = isValid && Validate(item, itemResults);

            results.AddRange(
                itemResults.Select(r => WithFullMemberName(r, item))
            );
        }

        return (isValid, results);
    }

    private static ValidationResult WithFullMemberName(ValidationResult result, ValidatableQuoteItem item) =>
        new(
            result.ErrorMessage,
            ["Items", item.Id.ToString(), ..result.MemberNames]
        );

    private static bool Validate(object validatable, List<ValidationResult> results)
    {
        var context = new ValidationContext(validatable);
        return Validator.TryValidateObject(validatable, context, results, true);
    }
}