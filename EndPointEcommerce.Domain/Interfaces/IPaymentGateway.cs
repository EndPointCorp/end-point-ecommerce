using EndPointEcommerce.Domain.Entities;

namespace EndPointEcommerce.Domain.Interfaces;

/// <summary>
/// Payment gateway service interface.
/// </summary>
public interface IPaymentGateway
{
    public class PaymentTransactionResult
    {
        public bool IsSuccess { get; set; }

        public string? TransactionId { get; set; }
        public string? ResponseCode { get; set; }
        public string? MessageCode { get; set; }
        public string? MessageDescription { get; set; }
        public string? AuthCode { get; set; }

        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }

        public static PaymentTransactionResult Success(
            string transactionId,
            string responseCode,
            string messageCode,
            string messageDescription,
            string authCode
        ) =>
            new()
            {
                IsSuccess = true,
                TransactionId = transactionId,
                ResponseCode = responseCode,
                MessageCode = messageCode,
                MessageDescription = messageDescription,
                AuthCode = authCode
            };

        public static PaymentTransactionResult Failure(string? errorCode = null, string? errorMessage = null) =>
            new()
            {
                IsSuccess = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
    }

    PaymentTransactionResult CreatePaymentTransaction(Order order);
}
