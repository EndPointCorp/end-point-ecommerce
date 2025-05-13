using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using EndPointCommerce.Domain.Entities;
using static EndPointCommerce.Domain.Interfaces.IPaymentGateway;

namespace EndPointCommerce.Infrastructure.Services.Payments;

/// <summary>
/// Calls the Authorize.NET "Create an Accept Payment Transaction" API endpoint.
/// https://developer.authorize.net/api/reference/index.html#accept-suite-create-an-accept-payment-transaction
/// </summary>
internal static class AuthorizeNetCreateTransaction
{
    private const int LINE_ITEM_MAX_LENGTH = 30;

    internal static PaymentTransactionResult Run(Order order)
    {
        var request = new createTransactionRequest
        {
            transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(), // charge the card

                amount = order.Total,
                payment = BuildPayment(order),
                billTo = BuildBillingAddress(order),
                lineItems = BuildLineItems(order)
            }
        };

        var controller = new createTransactionController(request);
        controller.Execute();

        var response = controller.GetApiResponse();

        return BuildResult(response);
    }

    private static paymentType BuildPayment(Order order) =>
        new()
        {
            Item = new opaqueDataType
            {
                dataDescriptor = order.PaymentMethodNonceDescriptor,
                dataValue = order.PaymentMethodNonceValue
            }
        };

    private static customerAddressType BuildBillingAddress(Order order) =>
        new()
        {
            firstName = order.BillingAddress.Name,
            lastName = order.BillingAddress.LastName,
            address = order.BillingAddress.Street,
            city = order.BillingAddress.City,
            zip = order.BillingAddress.ZipCode,
            state = order.BillingAddress.State?.Abbreviation,
            country = order.BillingAddress.Country!.Code
        };

    private static lineItemType[] BuildLineItems(Order order) =>
        order.Items.Select(item => new lineItemType
        {
            itemId = item.Id.ToString(),
            name = Truncate(item.Product.Name, LINE_ITEM_MAX_LENGTH),
            quantity = item.Quantity,
            unitPrice = item.UnitPrice
        }).ToArray();

    private static string Truncate(string text, int maxLength) =>
        text.Length > maxLength ? text[..maxLength] : text;

    private static PaymentTransactionResult BuildResult(createTransactionResponse? response)
    {
        if (response == null)
        {
            return PaymentTransactionResult.Failure(
                errorMessage: "No response from gateway"
            );
        }

        if (response.messages.resultCode != messageTypeEnum.Ok)
        {
            if (response.transactionResponse != null && response.transactionResponse.errors != null)
            {
                return PaymentTransactionResult.Failure(
                    response.transactionResponse.errors[0].errorCode,
                    response.transactionResponse.errors[0].errorText
                );
            }
            else
            {
                return PaymentTransactionResult.Failure(
                    response.messages.message[0].code,
                    response.messages.message[0].text
                );
            }
        }

        if (response.transactionResponse.messages == null)
        {
            if (response.transactionResponse.errors != null)
            {
                return PaymentTransactionResult.Failure(
                    response.transactionResponse.errors[0].errorCode,
                    response.transactionResponse.errors[0].errorText
                );
            }
            else
            {
                return PaymentTransactionResult.Failure(
                    errorMessage: "Unexpected format for error response from gateway"
                );
            }
        }

        return PaymentTransactionResult.Success(
            response.transactionResponse.transId,
            response.transactionResponse.responseCode,
            response.transactionResponse.messages[0].code,
            response.transactionResponse.messages[0].description,
            response.transactionResponse.authCode
        );
    }
}
